import os, sys
import numpy as np
import pandas as pd
import json
import requests

base_url = "https://api.genesysappliedresearch.com/v2/knowledge/knowledgebases"
orgId = "180dba95-1ab6-44b0-9c94-4630e8d280bf"

def make_q_json(question):
    q = {"query": question,
        "pageSize": 5,
        "pageNumber": 1,
        "sortOrder": "string",
        "sortBy": "string",
        "languageCode":"en-US",
        "documentType": "Faq"
        }
    return json.dumps(q)

def make_qa_json(question, answer):
    qa_dict = {"question": question,
      "answer": answer}
    return json.dumps(qa_dict)

def make_doc_json(question, answer):
    doc_dict = {"type": "faq",
       "faq": json.loads(make_qa_json(question, answer))}
    return json.dumps(doc_dict)

def get_kbs():

    querystring = {"limit":"5"}

    headers = {
        'Content-Type': "application/json",
        'organizationid': "180dba95-1ab6-44b0-9c94-4630e8d280bf",
        # 'token': "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJvcmdJZCI6IjE4MGRiYTk1LTFhYjYtNDRiMC05Yzk0LTQ2MzBlOGQyODBiZiIsImV4cCI6MTU3MTUyOTk4MywiaWF0IjoxNTcxNTI2MzgzfQ.JnwbKndyEI4XsO9bMfSKsA-euhDJ0xQ4mmzQtOJGqpk",
        'token': read_token(),
        'User-Agent': "PostmanRuntime/7.18.0",
        'Accept': "*/*",
        'Cache-Control': "no-cache",
        'Postman-Token': "b4a07672-1b97-4a53-881e-991bc4c48129,581b2b5d-b689-4c34-add9-d4cd57c61bdf",
        'Host': "api.genesysappliedresearch.com",
        'Accept-Encoding': "gzip, deflate",
        'Connection': "keep-alive",
        'cache-control': "no-cache"
    }

    response = requests.request("GET", base_url, headers=headers, params=querystring)

    # print(response.text)

    return json.loads(response.text)

def create_kbs(payload_str):
    url = "https://api.genesysappliedresearch.com/v2/knowledge/knowledgebases/"

    payload = payload_str
    headers = {
        'Content-Type': "application/json",
        'organizationid': "180dba95-1ab6-44b0-9c94-4630e8d280bf",
        'token': read_token(),
        'User-Agent': "PostmanRuntime/7.18.0",
        'Accept': "*/*",
        'Cache-Control': "no-cache",
        'Postman-Token': "824595a1-86bc-4145-ba07-671d4007cd29,67172d2c-1477-4117-9101-eabee2c2f18e",
        'Host': "api.genesysappliedresearch.com",
        'Accept-Encoding': "gzip, deflate",
        'Content-Length': "76",
        'Connection': "keep-alive",
        'cache-control': "no-cache"
    }

    response = requests.request("POST", url, data=payload, headers=headers)

    res_json = json.loads(response.text)

    return res_json, res_json['id']

def del_kb(kb_id):
    url = "https://api.genesysappliedresearch.com/v2/knowledge/knowledgebases/"+\
    kb_id

    headers = {
        'Content-Type': "application/json",
        'organizationid': "180dba95-1ab6-44b0-9c94-4630e8d280bf",
        'token': read_token(),
        'cache-control': "no-cache",
        'Postman-Token': "42f132af-1c94-4e8b-b9c9-56ab7f35b574"
    }

    response = requests.request("DELETE", url, headers=headers)

    print(response.text)


def get_doc(kb_id, doc_id):
    url = base_url+"/"+kb_id+\
    "/languages/en-US/documents/"+doc_id

    headers = {
        'Content-Type': "application/json",
        'organizationid': "180dba95-1ab6-44b0-9c94-4630e8d280bf",
        'token': "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJvcmdJZCI6IjE4MGRiYTk1LTFhYjYtNDRiMC05Yzk0LTQ2MzBlOGQyODBiZiIsImV4cCI6MTU3MTU0NTU4NywiaWF0IjoxNTcxNTQxOTg3fQ.28qryp8_2UKC1C03_t9fQnb6x2LWCwtVEwGf131TMyU",
        'User-Agent': "PostmanRuntime/7.18.0",
        'Accept': "*/*",
        'Cache-Control': "no-cache",
        'Postman-Token': "f7e225c4-4e65-4712-9ae3-4451683c2835,62bde82a-96ec-42ad-a8c4-327ce847b64a",
        'Host': "api.genesysappliedresearch.com",
        'Accept-Encoding': "gzip, deflate",
        'Connection': "keep-alive",
        'cache-control': "no-cache"
        }

    response = requests.request("GET", url, headers=headers)

    # print(response.text)

    return json.loads(response.text)


def get_docs(kb_id, specific_uri=None):
    url = base_url+"/"+kb_id+\
    "/languages/en-US/documents"

    if specific_uri is not None:
        url = specific_uri

    headers = {
    'Content-Type': "application/json",
    'token': read_token(),
    'organizationid': "180dba95-1ab6-44b0-9c94-4630e8d280bf",
    'User-Agent': "PostmanRuntime/7.18.0",
    'Accept': "*/*",
    'Cache-Control': "no-cache",
    'Postman-Token': "d5faced4-873e-4431-87d6-2fc5f09226bc,b58e5773-be9f-4d7c-ae0b-7e63cce5188b",
    'Host': "api.genesysappliedresearch.com",
    'Accept-Encoding': "gzip, deflate",
    'Connection': "keep-alive",
    'cache-control': "no-cache"
    }

    response = requests.request("GET", url, headers=headers)

    # print(response.text)

    return json.loads(response.text)


def get_all_docs(kb_id):
    result = get_docs(kb_id)
    new_result = result.copy()
    while new_result['nextUri'] is not None:
        new_result = get_docs(kb_id, new_result['nextUri'])
        result['entities'] = result['entities'] + new_result['entities']
        result['count'] = result['count'] + new_result['count']
    return result


def create_doc(kb_id, doc_str):

    url = base_url+"/"+kb_id+\
    "/languages/en-US/documents"

    payload = doc_str
    headers = {
        'Content-Type': "application/json",
        'organizationid': "180dba95-1ab6-44b0-9c94-4630e8d280bf",
        'token': read_token(),
        'cache-control': "no-cache",
        'Postman-Token': "03c7d0f1-0f29-4548-8670-f1ebeff8c0d0"
        }

    response = requests.request("POST", url, data=payload, headers=headers)

    # print(response.text)

    return json.loads(response.text)


def del_doc(kb_id, doc_id):
    url = base_url+"/"+kb_id+"/languages/en-US/documents/"+doc_id

    headers = {
        'Content-Type': "application/json",
        'organizationid': "180dba95-1ab6-44b0-9c94-4630e8d280bf",
        'token': read_token(),
        'cache-control': "no-cache",
        'Postman-Token': "4cb6ebb6-8fe0-4613-b1ed-3e4185d5d50c"
        }

    response = requests.request("DELETE", url, headers=headers)

    print(response.text)


def del_all_docs(kb_id):
    cont = True
    while cont:
        result = get_docs(kb_id)
        docs = result['entities']
        for doc in docs:
            del_doc(kb_id, doc['id'])
        if result['nextUri'] is None:
            cont = False


def train(kb_id):
    url = base_url+"/"+kb_id+\
    "/languages/en-US/trainings"

    headers = {
        'Content-Type': "application/json",
        'organizationid': "180dba95-1ab6-44b0-9c94-4630e8d280bf",
        'token': read_token(),
        'cache-control': "no-cache",
        'Postman-Token': "aa9f2a58-69ae-4ffd-9f1e-9685b7c60de9"
        }

    response = requests.request("POST", url, headers=headers)

    # print(response.text)

    return json.loads(response.text)

def get_trainings(kb_id):
    url = base_url+"/"+kb_id+\
    "/languages/en-US/trainings"

    querystring = {"limit":"2"}

    headers = {
        'Content-Type': "application/json",
        'organizationid': "180dba95-1ab6-44b0-9c94-4630e8d280bf",
        'token': read_token(),
        'cache-control': "no-cache",
        'Postman-Token': "0cb528ee-9f89-4765-8c00-75b334913d77"
        }

    response = requests.request("GET", url, headers=headers, params=querystring)

    # print(response.text)

    return json.loads(response.text)


def search(kb_id, question):
    url = base_url+"/"+kb_id+\
    "/search"

    headers = {
    'Content-Type': "application/json",
    'organizationid': "180dba95-1ab6-44b0-9c94-4630e8d280bf",
    'token': read_token(),
    'cache-control': "no-cache",
    'Postman-Token': "028f1529-2dd3-4121-ab37-4a487b8f0831"
    }

    response = requests.request("POST", url, data=question, headers=headers)

    # print(response.text)

    return json.loads(response.text)

def read_token():
    with open("token", "r") as f:
        return f.read()

def write_token(token):
    with open("token", "w") as f:
        f.write(token)