from flask import Flask, jsonify, render_template, request, redirect, url_for, make_response
import socket, random, time
import copy

# data = {
#     "roleplays":[
#         {"actions":[
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"}
#         ]},
#         {"actions":[
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"}
#         ]},
#         {"actions":[
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"}
#         ]},
#         {"actions":[
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"}
#         ]}]}



app = Flask(__name__)

gameData = list()
vitalData = list()

playersConut = 4
curPlayerCount = 0
players = dict()
# players = {0:"kaheh", 1:"mhd", 2:"xx", 3:"xrr"}

actionsConut = 2
episodeIndex = -1 
openEp = False

playersStarted = dict()
whoSubmit = dict()

vitalPlayer = 0 

episodeEnd = False

returnSubmit = 0

dead = list()



def newEpisode():
    global episodeIndex, gameData
    data=[{"type":"0"},{"type":"0"},{"type":"0"}]
    vitalPlayer = random.randint(0, playersConut-1)
    episodeIndex +=1
    episode = dict()
    gameData.append(episode)
    for i in dead:
        gameData[episodeIndex][i]=data
        whoSubmit[int(i)]=1

# =======================================================

@app.route('/reg', methods=['POST'])
def reg():
    global curPlayerCount, playersConut
    print("cpc:",curPlayerCount)
    print("pc:",playersConut)
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
    howm = request.json["howmplayerstarted"]
    if(not(int(data) in playersStarted)):
        playersStarted[int(data)]=1
    
    while(len(playersStarted.keys())==int(howm)):
        time.sleep(0.1)
        continue


    # while(not(len(playersStarted.keys())==playersConut)):
    #     # print(playersStarted)
    #     time.sleep(0.1)
    #     continue
    
    keys0 = list(players.keys())
    print("keys0::"+str(keys0))
    keys0.sort()
    print("sorted_keys::"+str(keys0))
    values = [players[i] for i in keys0]
    print(values)
    res = "/".join(values)
    print(res)
    return(res)




@app.route('/submit', methods=['POST'])
def submit():
    global whoSubmit, gameData, returnSubmit, episodeIndex
    pindex = request.json['pindex']
    data = request.json['data']
    
    print("ep:",episodeIndex)
    print("pind:",pindex)
    print("data:",data)


    gameData[episodeIndex][pindex]=data
    whoSubmit[int(pindex)]=1
    while(not(len(whoSubmit.keys())==playersConut)):
        # print(len(whoSubmit.keys()))
        time.sleep(0.1)
        continue
    


    res = copy.deepcopy(gameData[episodeIndex])
    keys1 = list(res.keys())
    keys1.sort()
    res2 = dict()
    res2["roleplays"]=[]
    for key in keys1:
        for i in res[key]:
            # print(i)
            if(i["type"]=="move"):
                i["ser"]=i["target"]
                del i["target"]
            elif(i["type"]=="fire"):
                if(i["gunType"]=="Sword"):
                    i["ser"]=i["gunType"]
                    del i["gunType"]
                else:
                    i["ser"] = i["gunType"]+"/"+i["target"]
                    del i["gunType"]
                    del i["target"]
            
        temp0 = {"actions": res[key] }
        res2["roleplays"].append(temp0)
    
    
    returnSubmit+=1
    if(returnSubmit==4):
        returnSubmit = 0
        whoSubmit = dict()
        newEpisode()
    
    print(res2)
    return(jsonify(res2))




@app.route('/vital', methods=['POST'])
def vital():
    global vitalData
    data = json.loads(request.json["data"])
    vitalData.append(data)
    newEpisode()  #create new episodeIndex
    res = '{"res":"ok"}'
    return jsonify(res)


@app.route('/lose', methods=['POST'])
def lose():
    global vitalData
    pindex = request.json['pindex']
    if(not(pindex in dead)):
        dead.append(pindex)
        print("lose",pindex)
    if(len(dead)>=(playersConut-1)):
        print("winner")
        reset()
    res = '{"res":"ok"}'
    return jsonify(res)



def reset():
    global gameData, vitalData, playersConut
    global curPlayerCount, players, actionsConut
    global episodeIndex, openEp, playersStarted
    global whoSubmit, vitalPlayer, episodeEnd, dead

    gameData = list()
    vitalData = list()

    playersConut = 4
    curPlayerCount = 0
    players = dict()

    actionsConut = 3
    episodeIndex = -1 
    openEp = False

    playersStarted = dict()
    whoSubmit = dict()

    vitalPlayer = 0 

    episodeEnd = False

    returnSubmit = 0

    dead = list()
    newEpisode()




if __name__ == '__main__':
    newEpisode()
    app.run(threaded=True, debug=False, host='0.0.0.0', port=5000) 


