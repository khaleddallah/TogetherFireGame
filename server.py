
from flask import Flask, jsonify, render_template, request, redirect, url_for, make_response
import socket, random

app = Flask(__name__)


class Action(object):
    def __init__(self, atype="0", gunType="0", target="0"):
        self.atype = atype
        self.gunType = gunType
        self.target = target

class Roleplay(object):
    def __init__(self, actions):
        self.actions = actions

class Episode(object):
    def __init__(self, roleplays):
        self.roleplays = roleplays

class VitalData(object):
    def __init__(self, health, golds):
        self.health = health
        self.golds = golds

class GameData(object):
    def __init__(self):
        self.episodes = episodes
        self.vitalDatas = vitalDatas


gameData = GameData()
playersConut = 4
episodeIndex = 0 


def initialGame():
    for i in range(playersConut):
        gameData.vitalDatas = 

@app.route('/pdata', methods=['GET'])
def home():
    global data
    return jsonify(data)


@app.route('/submit', methods=['POST'])
def reroute():
    print(request.json['pindex'])
    print(request.json['data'])
    return("Good")




app.run(debug=False, host='127.0.0.1', port=5000) 


