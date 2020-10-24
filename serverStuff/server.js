// JavaScript source code
var app = require('express')();
var server = require('http').Server(app);
var io = require('socket.io')(server);

const PORT = process.env.PORT || 3000;

app.get('/', function (req, res) {
    res.sendFile(__dirname + '/index.html');
});

var Users = new Map();

var Deck = [];
var DiscardPile = [];

var host = "";

io.sockets.on('connection', (socket) => {
    console.log('a user connected');

    socket.emit('connectionmessage', {
        id: socket.id
    });

    addUser(socket);

    socket.on('updateUsername', (newName) => {
        addUsername(newName);
    });

    socket.on('disconnect', () => {
        console.log('user disconnected');
        io.emit('removeUser', { id: socket.id });
        removeUser(socket);
    });

    socket.on('getUsernames', () => {
        let tempUsers = Array.from(Users.values());
        var usernameObject = {};
        for (var i = 0; i < tempUsers.length; i++) {
            usernameObject[i] = {
                username: tempUsers[i]["username"],
                id: tempUsers[i]["id"]
            };
        }
        socket.emit('allUsers', usernameObject);
    });

    socket.on('loadGame', () => {
        console.log('setting deck...')
        resetDeck();
        io.emit('loadGameScene');
    });

    socket.on('playCard', (card) => {   //sending card played to everyone
        console.log(Users[socket.id].username + ' played a ' + card + ' card');
        io.emit('cardPlayed', { card: card, id: socket.id });
    });

    socket.on('drawCard', () => {
        console.log(Users[socket.id].username + ' drew a card');
        socket.emit('drewCard', { card: drawCard() });
        //maybe send something to everyone else updating hand count
    });

    socket.on('discardCard', (card) => {
        DiscardPile.push(card);
    });

    socket.on('loadHands', () => {
        console.log('dealing cards to all players');
        handsToAllPlayers();
    });




    function handsToAllPlayers() {
        Users.forEach((value, key, map) => {
            console.log('dealing cards to player: ' + key);
            drawHand(key);
        });
    }

    function drawHand(id) {
        io.to(id).emit('newHand', { card1: drawCard(), card2: drawCard(), card3: drawCard() });
    }

    function drawCard() {
        if (Deck.length == 0) discardToDeck();

        var rand = Math.floor(Math.random() * Deck.length);
        var card = Deck[rand];

        var swap = Deck[rand];
        Deck[rand] = Deck[Deck.length - 1];
        Deck[Deck.length - 1] = swap;
        Deck.pop();

        return card;
    }

    function resetDeck() {
        // sets the deck back to default, and emptys the discard
        DiscardPile = [];
        Deck = ['Axolotl1', 'Axolotl2', 'Axolotl3', 'Axolotl4', 'Dino1', 'Dino2', 'Dino3', 'Dino4', 'Dragon1', 'Dragon2', 'Dragon3', 'Dragon4', 'Frog1', 'Frog2', 'Frog3', 'Frog4', 'Gator1', 'Gator2', 'Gator3', 'Gator4', 'Lizard1', 'Lizard2', 'Lizard3', 'Lizard4', 'Axolotl1', 'Axolotl2', 'Axolotl3', 'Axolotl4', 'Dino1', 'Dino2', 'Dino3', 'Dino4', 'Dragon1', 'Dragon2', 'Dragon3', 'Dragon4', 'Frog1', 'Frog2', 'Frog3', 'Frog4', 'Gator1', 'Gator2', 'Gator3', 'Gator4', 'Lizard1', 'Lizard2', 'Lizard3', 'Lizard4', 'Limit1', 'Limit2', 'Limit3', 'Limit4', 'Limit5', 'Limit6'];
        console.log('deck is ready')
    }

    function discardToDeck() {
        if (Deck.length == 0) Deck = DiscardPile;
        else DiscardPile.forEach((value, index, array) => { // might only need value
            Deck.push(value);
        });

        DiscardPile = [];
    }



    function changeUserProperty(property, value) {
        // users properties: id, username, observeallcontrol, observeallevents
        if (Users.has(socket.id)) {
            tempObj = Users.get(socket.id);
            // console.log('changed current user property: ' + property);
            tempObj[property] = value;
            Users.set(socket.id, tempObj);
        }
        checkUsers();
    }

    function addUsername(newUsername) {
        // coming back to this
        changeUserProperty('username', newUsername);
    }

    function addUser(socket) {
        // create a new user mapping
        if (!Users.has(socket.id)) {
            Users.set(
                socket.id,
                {
                    username: "Name",
                    id: socket.id
                }
            );

            console.log(Array.from(Users.keys()).length);
            if (Array.from(Users.keys()).length == 1) {
                host = socket.id;
                //console.log(Users[socket.id].username + ' is now the host.');
                socket.emit('host');
            }

            checkUsers();
        }
    };

    function removeUser(socket) {
        if (Users.has(socket.id)) {
            if (host == socket.id && Users.length > 1) {
                var user = Array.from(Users.keys())[0];
                if (user == socket.id) user = Array.from(Users.keys())[1];

                host = user;
                //console.log(User[user].username + ' is now the host.');
                io.to(user).emit('host');
            }
            else if (host == socket.id) {
                host = "";
            }

            Users.delete(socket.id);
            checkUsers();
        }
    };

    function checkUsers() {
        console.table(Users);
        listUsers();
    };

    function listUsers() {

        // Giving id, name pairs
        let tempUsers = Array.from(Users.values());

        var usernameObject = {};
        for (var i = 0; i < tempUsers.length; i++) {
            usernameObject[i] = {
                username: tempUsers[i]["username"],
                id: tempUsers[i]["id"]
            };
        }
        io.emit('users', usernameObject);
    }
});

server.listen(PORT);