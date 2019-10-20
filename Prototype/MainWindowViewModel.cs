using Newtonsoft.Json.Linq;
using Prototype.Interfaces;
using Prototype.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.Integration;
using System.Windows.Input;

namespace Prototype
{
    public class UnansweredQuestion : ObservableObject
    {
        public UnansweredQuestion(int id)
        {
            Id = id;
            AltQuestionIds.CollectionChanged += OnAltQuestionIdsCollectionChanged;
        }

        private void OnAltQuestionIdsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) =>
            AltQuestionIdsString = string.Join(", ", AltQuestionIds.ToList());

        public int Id { get; }

        private string _mainQuestion;
        public string MainQuestion
        {
            get => _mainQuestion;
            set => SetPropertyField(ref _mainQuestion, value);
        }

        private int _count;
        public int Count
        {
            get => _count;
            set => SetPropertyField(ref _count, value);
        }

        public ObservableCollection<int> AltQuestionIds { get; } = new ObservableCollection<int>();

        private string _altQuestionIdsString;
        public string AltQuestionIdsString
        {
            get => _altQuestionIdsString;
            set => SetPropertyField(ref _altQuestionIdsString, value);
        }

        public List<string> AnswerIds { get; set; }
        public int PlotId { get; set; }
    }

    public class Answer
    {
        public Answer(string id) => Id = id;

        public string Id { get; }

        public double Confidence { get; set; }

        public string Text { get; set; }
    }

    public class MainWindowViewModel : ObservableObject
    {
        public ObservableCollection<UnansweredQuestion> UnansweredQuestions { get; } =
            new ObservableCollection<UnansweredQuestion>();

        public ObservableCollection<Answer> Answers { get; } =
            new ObservableCollection<Answer>();

        private string _question;
        public string Question
        {
            get => _question;
            set => SetPropertyField(ref _question, value);
        }

        public ICommand SendQueryCommand { get; }

        public WindowsFormsHost LinePlotHost { get; }

        public MainWindowViewModel(IKnowledgeBase kb)
        {
            _kb = kb ?? throw new ArgumentNullException(nameof(kb));

            InitializeLinePlot();
            LinePlotHost = new WindowsFormsHost {Child = _linePlot};

            SendQueryCommand = new RelayCommand(SendQuery, CanSendQuery);
        }

        private void InitializeLinePlot()
        {
            _linePlot = new Chart { Dock = DockStyle.Fill };

            _linePlot.Series.Add(new Series(LinePlotSeriesName) { ChartType = SeriesChartType.Line });
            _linePlot.ChartAreas.Add(new ChartArea());

            ResetLinePlot();
        }

        private void ResetLinePlot()
        {
            _hist.Clear();
            _hist.Add(0, 0);
            UpdateLinePlot();
        }

        private void UpdateLinePlot()
        {
            // set axis ranges
            const double defaultXAxisMaximum = 10;
            const double defaultYAxisMaximum = 5;

            var xAxisMaximum = Math.Max(_hist.Keys.Count, defaultXAxisMaximum);
            var yAxisMaximum = Math.Max(_hist.Values.Count != 0 ? _hist.Values.Max() : 0, 0) + defaultYAxisMaximum;

            _linePlot.ChartAreas[0].AxisY.Maximum = yAxisMaximum;
            _linePlot.ChartAreas[0].AxisX.Minimum = 0;
            _linePlot.ChartAreas[0].AxisX.Maximum = xAxisMaximum;

            _linePlot.DataSource = _hist;
            _linePlot.Series[LinePlotSeriesName].XValueMember = "Key";
            _linePlot.Series[LinePlotSeriesName].YValueMembers = "Value";

            _linePlot.Refresh();
        }

        private bool CanSendQuery() => !string.IsNullOrWhiteSpace(Question) && !_busy;

        private async void SendQuery()
        {
            if (!CanSendQuery()) return;

            try
            {
                _busy = true;
                var results = JObject.Parse(_kb.Search(Question).Content)["results"];

                var answers = await Task.Run(() => results.Select(r =>
                        new Answer(r["id"].ToString())
                        {
                            Confidence = double.Parse(r["confidence"].ToString()),
                            Text = r["faq"]["answer"].ToString()
                        }).ToList()
                );

                Answers.Clear();
                answers.ForEach(Answers.Add);

                if (IsNewQuestion())
                {
                    var q = new UnansweredQuestion(UnansweredQuestions.Count + 1)
                    {
                        MainQuestion = Question,
                        Count = 1,
                        AnswerIds = Answers.Select(x => x.Id).ToList()
                    };

                    var duplicates = FindDuplicates(q).ToList();
                    if (duplicates.Any())
                    {
                        var maxCount = duplicates.Max(x => x.Count);
                        var entryWithMaxCount = duplicates[duplicates.FindIndex(x => x.Count == maxCount)];
                        ++entryWithMaxCount.Count;
                        entryWithMaxCount.AltQuestionIds.Add(q.Id);
                        q.AltQuestionIds.Add(entryWithMaxCount.Id);

                        _hist[entryWithMaxCount.PlotId]++;
                    }
                    else
                    {
                        ++_plotIdCounter;
                        _hist.Add(_plotIdCounter, 1);
                    }

                    q.PlotId = _plotIdCounter;
                    UnansweredQuestions.Add(q);

                    UpdateLinePlot();
                }
            }
            //catch (Exception)
            //{
            //    // ignored
            //}
            finally
            {
                _busy = false;
            }
        }

        private IEnumerable<UnansweredQuestion> FindDuplicates(UnansweredQuestion q)
        {
            var dups = new List<UnansweredQuestion>();

            if (UnansweredQuestions.Count == 0) return dups;

            const int similarityCountThreshold = 3;
            foreach (var existingQuestion in UnansweredQuestions)
            {
                var count = existingQuestion.AnswerIds.Where((t, i) => t == q.AnswerIds[i]).Count();
                if (count >= similarityCountThreshold)
                {
                    dups.Add(existingQuestion);

                    if (existingQuestion.Count > 1)
                        break;
                }
            }
            return dups;
        }

        private bool IsNewQuestion()
        {
            if (Answers.Count > 1)
            {
                const double scoreDiffThreshold = 0.5;
                return Answers[0].Confidence - Answers[1].Confidence < scoreDiffThreshold;
            }

            if (Answers.Count == 1)
            {
                const double scoreThreshold = 0.57;
                return Answers[0].Confidence < scoreThreshold;
            }

            return true;
        }

        private readonly IKnowledgeBase _kb;
        private bool _busy;

        private const string LinePlotSeriesName = "series";
        private Chart _linePlot;
        private readonly Dictionary<int, int> _hist = new Dictionary<int, int>();
        private int _plotIdCounter;
    }
}
