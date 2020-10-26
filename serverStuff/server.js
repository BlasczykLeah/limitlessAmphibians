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
var Limits = [];
var WinCards = [];

var whosTurn = -1;
//var host = "";

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

    socket.on('playCard', (data) => {   //sending card played to everyone
        if (Array.from(Users.keys())[whosTurn] == socket.id) {
            //console.log(Users[socket.id].username + ' played a ' + card + ' card');
            io.emit('cardPlayed', { card: data.card, id: socket.id, targetID: data.targetID, targetCard: data.targetCard });
        }
        //else console.log(Users[socket.id].username + ' cannot play a card, its not their turn');
    });

    socket.on('drawCard', () => {
        //console.log(Users[socket.id].username + ' drew a card');
        socket.emit('drewCard', { card: drawCard() });
        //maybe send something to everyone else updating hand count
    });

    socket.on('discardCard', (card) => {
        DiscardPile.push(card);
        console.table(DiscardPile);
        //socket.broadcast.emit('cardRemoved', { id: socket.id, card: card });
    });

    socket.on('loadHands', () => {
        console.log('dealing cards to all players');
        handsToAllPlayers();

        console.log('choosing who goes first...');
        //whosTurn = Math.floor(Math.random * Users.keys().length);
        //turn();
    });

    socket.on('firstTurn', () => {
        whosTurn = 0;
        turn();
    });

    socket.on('nextTurn', () => {
        var newCard = drawCard();
        console.log(Array.from(Users.keys())[whosTurn]);
        io.to(Array.from(Users.keys())[whosTurn]).emit('drewCard', { card: newCard });
        turn();
    });

    socket.on('setValues', (thing) => {
        changeUserProperty('limit', thing.limit);
        changeUserProperty('win', thing.wincon);
        socket.broadcast.emit('playerValues', { id: socket.id, limit: thing.limit, win: thing.wincon });
    });

    socket.on('win', (id) => {
        io.emit('winscene', { id: id } );
    });

    socket.on('newWinCard', (id) => {
        var win = drawWin();
        io.emit('playerValues', {limit: 'none', win: win });
    });




    function turn() {
        var sockets = Array.from(Users.keys());
        whosTurn = whosTurn + 1;
        if (whosTurn == sockets.length) whosTurn = 0;

        console.log('index ' + whosTurn + ' player`s turn');
        io.emit('playerTurn', { id: sockets[whosTurn] });
    }

    function handsToAllPlayers() {
        Users.forEach((value, key, map) => {
            console.log('dealing cards to player: ' + key);
            drawHand(key);
        });

            // put leftover Limit cards in Deck
        for (var i = 0; i < Limits.length; i++) {
            Deck.push(Limits.pop());
        }

        console.log('cards have been dealt');
    }

    function drawHand(id) {
        var limit = drawLimit();
        var win = drawWin();

        var card1 = drawCard();
        var card2 = drawCard();
        var card3 = drawCard();

        

        io.to(id).emit('newHand', { card1: card1, card2: card2, card3: card3, limit: limit, win: win });
    }

    function drawLimit() {
        if (Limits.length > 0) {
            var rand = Math.floor(Math.random() * Limits.length);

            var swap = Limits[rand];
            Limits[rand] = Limits[Limits.length - 1];
            Limits[Limits.length - 1] = swap;

            var card = Limits.pop();

            return card;
        }
        else console.log('no limit cards found.');
    }

    function drawWin() {    // win cards are no longer unique; everyone *could* get the same one
        var rand = Math.floor(Math.random() * WinCards.length);
        return WinCards[rand];
    }

    function drawCard() {
        if (Deck.length == 0) {
            discardToDeck();
            console.table(DiscardPile);
            console.table(Deck);
        }

        var rand = Math.floor(Math.random() * Deck.length);

        var swap = Deck[rand];
        Deck[rand] = Deck[Deck.length - 1];
        Deck[Deck.length - 1] = swap;

        var card = Deck.pop();

        return card;
    }

    function resetDeck() {
        // sets the deck back to default, and emptys the discard
        DiscardPile = [];
        Deck = ['Axolotl1', 'Axolotl2', 'Axolotl3', 'Axolotl4', 'Dino1', 'Dino2', 'Dino3', 'Dino4', 'Dragon1', 'Dragon2', 'Dragon3', 'Dragon4', 'Frog1', 'Frog2', 'Frog3', 'Frog4', 'Gator1', 'Gator2', 'Gator3', 'Gator4', 'Lizard1', 'Lizard2', 'Lizard3', 'Lizard4', 'Axolotl5', 'Axolotl6', 'Axolotl7', 'Axolotl8', 'Dino5', 'Dino6', 'Dino7', 'Dino8', 'Dragon5', 'Dragon6', 'Dragon7', 'Dragon8', 'Frog5', 'Frog6', 'Frog7', 'Frog8', 'Gator5', 'Gator6', 'Gator7', 'Gator8', 'Lizard5', 'Lizard6', 'Lizard7', 'Lizard8', 'Cat', 'Box', 'Magic1', 'Magic2', 'Magic3', 'Magic4', 'Magic5', 'Magic6', 'Magic7', 'Magic8', 'Magic9', 'Magic10', 'Magic11', 'Magic12'];
        Limits = ['Limit1', 'Limit2', 'Limit3', 'Limit4', 'Limit5', 'Limit6', 'Limit1', 'Limit2', 'Limit3', 'Limit4', 'Limit5', 'Limit6'];
        WinCards = ['Win1', 'Win2', 'Win3', 'Win4', 'Win5', 'Win6', 'Win7', 'Win8', 'Win9', 'Win10', 'Win11', 'Win12', 'Win13', 'Win14', 'Win15', 'Win16', 'Win17'];
        console.log('deck is ready');
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
                    id: socket.id,
                    limit: "none",
                    win: "none"
                }
            );

            console.log(Array.from(Users.keys()).length);
            if (Array.from(Users.keys()).length == 1) {
                //host = socket.id;
                //console.log(Users[socket.id].username + ' is now the host.');
                socket.emit('host');
            }

            checkUsers();
        }
    };

    function removeUser(socket) {
        if (Users.has(socket.id)) {
            /*if (host == socket.id && Users.length > 1) {
                var user = Array.from(Users.keys())[0];
                if (user == socket.id) user = Array.from(Users.keys())[1];

                //host = user;
                //console.log(User[user].username + ' is now the host.');
                io.to(user).emit('host');
            }
            else if (host == socket.id) {
                //host = "";
            }*/

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