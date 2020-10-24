// JavaScript source code
var app = require('express')();
var server = require('http').Server(app);
var io = require('socket.io')(server);

const PORT = process.env.PORT || 3000;

app.get('/', function (req, res) {
    res.sendFile(__dirname + '/index.html');
});

var HighScores = new Map();  // declaring Users structure


io.sockets.on('connection', (socket) => {
    console.log('a user connected');

    socket.emit('connectionmessage', {
        id: socket.id
    });

    socket.on('LoadTheScore', (place) => {

        var score = GetScore(place);
        console.log(score.key + " | " + score.value);

        if (score.value > 0) {
            console.log('sending score...');

            io.emit('LoadScore', {
                name: score.key,
                score: score.value
            });
        }

        if (place == 5) {
            io.emit('ScoresUpdated');
        }
    });

    socket.on('EnterScore', (score) => {

        HighScores.set(
            score.name,
            score.score
        );

        if (HighScores.size > 1) {
            var temp = new Map([...HighScores.entries()].sort((a, b) => b.value - a.value));
            HighScores = temp;
        }
        console.log(score.name + ' | ' + HighScores.get(score.name) + ' saved.');
    });

    socket.on('ResetScores', () => {
        HighScores = new Map();
        console.log('High scores have been reset.');
    });

    socket.on('disconnect', () => {
        console.log('user disconnected');
    });

    function GetScore(placement) {
        // 1 = 1st, 2 = 2nd...
        if (HighScores.size >= placement) {
            var key = Array.from(HighScores.keys())[placement - 1];
            return {
                key: key,
                value: HighScores.get(key)
            };
        }
        else {
            return {
                key: 'none',
                value: -1
            };
        }
    }
});

server.listen(PORT);