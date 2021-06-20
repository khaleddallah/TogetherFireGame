from flask import Flask, jsonify, render_template, request, redirect, url_for, make_response
import socket, random, time

app = Flask(__name__)

gameData = list()
vitalData = list()

playersConut = 4
curPlayerCount = 0
players = dict()
# players = {0:"kaheh", 1:"mhd", 2:"xx", 3:"xrr"}

actionsConut = 4
episodeIndex = -1 
openEp = False

playersStarted = dict()

vitalPlayer = 0 

episodeEnd = False


def newEpisode():
    global episodeIndex, gameData
    vitalPlayer = random.randint(0, playersConut-1)
    episodeIndex +=1
    episode = dict()
    gameData.append(episode)


# =======================================================

@app.route('/reg', methods=['POST'])
def reg():
    global curPlayerCount, playersConut
    if(curPlayerCount<playersConut):
        data = request.json["name"]
        print("name:"+data)
        if not(data in players.values()):
            players[curPlayerCount] = data
            res = curPlayerCount 
            curPlayerCount+=1
        else:
            res = -2
    else:
        res = -1
    return(str(res))



@app.route('/isstarted', methods=['POST'])
def isStarted():
    global playersStarted, players
    data = request.json["index"]
    playersStarted[int(data)]=1
    while(not(len(playersStarted.keys())==playersConut)):
        print(data)
        print(playersStarted)
        time.sleep(1)
        continue
    res = "y/"
    keys0 = list(players.keys())
    print("keys0::"+str(keys0))
    keys0.sort()
    print("sorted_keys::"+str(keys0))
    values = [players[i] for i in keys0]
    print(values)
    res += "/".join(values)
    print(res)
    return(res)




@app.route('/submit', methods=['POST'])
def submit():
    pindex = request.json['pindex']
    data = request.json['data']
    gameData[episodeIndex][pindex]=data
    print(gameData)
    return("ok")



@app.route('/pdata', methods=['POST'])
def pdata():
    global gameData, episodeIndex, vitalPlayer
    data = gameData[episodeIndex].copy()

    # send the vitalplayer (random)
    data['isVital'] = int( int(request.json['index']) ==  vitalPlayer )
    return jsonify(data)


@app.route('/vital', methods=['POST'])
def vital():
    global vitalData
    data = json.loads(request.json["data"])
    vitalData.append(data)
    newEpisode()  #create new episodeIndex
    res = '{"res":"ok"}'
    return jsonify(res)




if __name__ == '__main__':
    newEpisode()
    app.run(threaded=True, debug=False, host='0.0.0.0', port=5000) 


