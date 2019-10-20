using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Prototype
{
    public class UnansweredQuestion : ObservableObject
    {
        public UnansweredQuestion(int id)
        {
            Id = id;
            AltQuestionIds.CollectionChanged += OnAltQuestionIdsCollectionChanged;
        }

        private void OnAltQuestionIdsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var x = AltQuestionIds.ToList();
            AltQuestionIdsString = string.Join(", ", AltQuestionIds.ToList());
        }

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
    }

    public class Answer
    {
        public Answer() => Id = Guid.NewGuid();

        public Guid Id { get; }

        public double ConfidenceLevel { get; set; }

        public string Text { get; set; }
    }

    public class MainWindowViewModel : ObservableObject
    {
        public ObservableCollection<UnansweredQuestion> UnansweredQuestions { get; } =
            new ObservableCollection<UnansweredQuestion>();
        public ObservableCollection<Answer> Answers { get; } =
            new ObservableCollection<Answer>();

        public void Update()
        {
            UnansweredQuestions.Add(
                new UnansweredQuestion(1)
                {
                    MainQuestion = "what up?",
                    Count = 5
                });
            UnansweredQuestions.First().AltQuestionIds.Add(2);
            UnansweredQuestions.First().AltQuestionIds.Add(3);
            UnansweredQuestions.First().AltQuestionIds.Add(4);
            UnansweredQuestions.First().AltQuestionIds.Add(5);

            UnansweredQuestions.Add(new UnansweredQuestion(2) {MainQuestion = "yo", Count = 1});
            UnansweredQuestions.Add(new UnansweredQuestion(3) {MainQuestion = "sup", Count = 1});
            UnansweredQuestions.Add(new UnansweredQuestion(4) {MainQuestion = "how are you", Count = 1});
            UnansweredQuestions.Add(new UnansweredQuestion(5) {MainQuestion = "yo", Count = 1});

            UnansweredQuestions.Skip(1).First().AltQuestionIds.Add(1);
            UnansweredQuestions.Skip(2).First().AltQuestionIds.Add(1);
            UnansweredQuestions.Skip(3).First().AltQuestionIds.Add(1);
            UnansweredQuestions.Skip(4).First().AltQuestionIds.Add(1);
        }
    }
}
