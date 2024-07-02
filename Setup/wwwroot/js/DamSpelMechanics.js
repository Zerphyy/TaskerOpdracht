
var connection;
var isUpdating = false;
document.addEventListener('DOMContentLoaded', function () {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/gameHub")
        .build();
    connection.on("PlayerMoved", function (gameState) {
        console.log("hallo!");
        $.ajax({
            url: '/Dammen/UpdateBoardData',
            method: 'POST',
            dataType: 'json',
            data: {
                gameState: gameState,
                gameId: CheckersModule.getGameId()
            },
            success: function (result) {
                if (result.success) {
                    UpdateBoard(gameState);
                } else {
                    console.log("could not save to database");
                }
                isUpdating = false; // set the flag to false after the update
            },
            error: function (error) {
                console.error('Error:', error);
                isUpdating = false; // set the flag to false after the update
            }
        });
        
    })
    connection.start().then(function () {
        console.log("SignalR Connected");
    }).catch(function (err) {
        console.error(err.toString());
    });
});
var CheckersModule = (function () {
    var gameStateArray = [];
    var selectedPiece = null;
    var gameState = null;
    var gebruiker = null;
    var spelers = null;
    var gameId = null;
    var captureMoves = [];
    function updateGameStateArray(gameState) {
        for (let j = 0; j < 8; j++) {
            gameStateArray[j] = [];
            for (let k = 0; k < 8; k++) {
                gameStateArray[j][k] = parseInt(gameState.charAt(j * 8 + k));
            }
        }
    }
    function GameStateArrayToGameState() {
        // Flatten the 2D array and join it into a string
        return gameStateArray.flat().join('');
    }
    function createCircleDiv() {
        var circleDiv = document.createElement('div');
        circleDiv.style.width = '50%';
        circleDiv.style.height = '50%';
        circleDiv.style.borderRadius = '50%';
        circleDiv.style.position = 'relative';
        circleDiv.style.left = '25%';
        circleDiv.style.top = '25%';
        circleDiv.style.backgroundColor = 'grey';
        circleDiv.className = 'grey-circle';
        return circleDiv;
    }

    function addMoveListener(i, squareDiv, isCapture) {
        var circleDiv = createCircleDiv();
        circleDiv.addEventListener('click', function () {
            movePiece(i, squareDiv, isCapture);
        });
        squareDiv.appendChild(circleDiv);
    }

    function checkMove(index, i, isCapture) {
        if (index !== null && index.row >= 0 && index.row < 8 && index.col >= 0 && index.col < 8 && gameStateArray[index.row][index.col] === 0) {
            var squareDiv = CheckersModule.getShadowRoot().querySelector('#square' + (index.row * 8 + index.col));
            addMoveListener(i, squareDiv, isCapture);
            return { "squareDiv": squareDiv, "isCapture": true };
        }
    }

    function getPossibleMoves(i, gameState, gebruiker, spelers) {
        var gameStateArray = CheckersModule.getGameStateArray();
        var possibleMoves = [];

        var square = document.querySelector('#shadowHost').shadowRoot.querySelector('#square' + i);
        var piece = square.firstChild;
        var row = Math.floor(i / 8);
        var col = i % 8;
        var leftSquareIndex, rightSquareIndex;
        if (gebruiker === spelers[0]) {
            leftSquareIndex = col === 0 ? null : { row: row + 1, col: col - 1 };
            rightSquareIndex = col === 7 ? null : { row: row + 1, col: col + 1 };
        } else {
            leftSquareIndex = col === 0 ? null : { row: row - 1, col: col - 1 };
            rightSquareIndex = col === 7 ? null : { row: row - 1, col: col + 1 };
        }

        CheckersModule.removeAvailableMovePositions();
        [leftSquareIndex, rightSquareIndex].forEach(function (index) {
            if (index !== null) {
                var move = checkMove(index, i, false);
                if (move) {
                    possibleMoves.push(move);
                }
            }
        });

        if (gebruiker === spelers[0]) {
            leftSquareIndex = col <= 1 ? null : { row: row + 2, col: col - 2 };
            rightSquareIndex = col >= 6 ? null : { row: row + 2, col: col + 2 };
        } else {
            leftSquareIndex = col >= 6 ? null : { row: row - 2, col: col + 2 };
            rightSquareIndex = col <= 1 ? null : { row: row - 2, col: col - 2 };
        }

        [leftSquareIndex, rightSquareIndex].forEach(function (index) {
            if (index !== null) {
                var enemyRow = (row + index.row) / 2;
                var enemyCol = (col + index.col) / 2;
                if (gameStateArray[enemyRow] && gameStateArray[enemyRow][enemyCol] !== 0 && !CheckersModule.isPlayersPiece(gameStateArray[enemyRow][enemyCol], gebruiker, spelers)) {
                    var move = checkMove(index, i, true);
                    if (move) {
                        possibleMoves.push(move);
                    }
                }
            }
        });
        return possibleMoves;
    }

    function movePiece(i, squareDiv, isCapture) {
        if (isUpdating) {
            console.log('Game state is updating, please wait...');
            return;
        }

        isUpdating = true; // set the flag to true when starting the update
        CheckersModule.removeAvailableMovePositions();

        let square = CheckersModule.getSelectedPiece();
        let piece = square.firstChild;
        let pieceState = gameStateArray[Math.floor(i / 8)][i % 8];
        square.removeChild(piece);
        squareDiv.appendChild(piece);
        piece.removeEventListener('click', piece.clickHandler);

        i = Array.from(squareDiv.parentNode.children).indexOf(squareDiv);

        gameStateArray[Math.floor(square.id.slice(6) / 8)][square.id.slice(6) % 8] = 0;
        gameStateArray[Math.floor(i / 8)][i % 8] = pieceState;

        if (isCapture) {
            var enemyRow = (Math.floor(square.id.slice(6) / 8) + Math.floor(i / 8)) / 2;
            var enemyCol = (square.id.slice(6) % 8 + i % 8) / 2;
            gameStateArray[enemyRow][enemyCol] = 0;
            var enemySquare = document.querySelector('#shadowHost').shadowRoot.querySelector('#square' + (enemyRow * 8 + enemyCol));
            enemySquare.removeChild(enemySquare.firstChild);
        }

        CheckersModule.setGameStateArray(gameStateArray.flat().join(''));

        var clickHandler = function () {
            CheckersModule.setSelectedPiece(squareDiv);
            getPossibleMoves(i, CheckersModule.getGameState(), CheckersModule.getGebruiker(), CheckersModule.getSpelers());
        };
        piece.addEventListener('click', clickHandler);
        piece.clickHandler = clickHandler;

        CheckersModule.setSelectedPiece(piece);
        CheckersModule.setGameState(CheckersModule.GameStateArrayToGameState());
        connection.invoke("NotifyPlayerMoved", CheckersModule.getGameState()).catch(function (err) {
            console.error(err.toString());
        });
        checkForAdditionalCapture(squareDiv, isCapture);
    }

    return {
        getGameStateArray: function () {
            return gameStateArray;
        },
        setGameStateArray: function (gameState) {
            updateGameStateArray(gameState);
        },
        getSelectedPiece: function () {
            return selectedPiece;
        },
        setSelectedPiece: function (piece) {
            selectedPiece = piece;
        },
        getShadowRoot: function () {
            return document.querySelector('#shadowHost').shadowRoot;
        },
        getAvailableMovePositions: function () {
            return document.querySelector('#shadowHost').shadowRoot.querySelectorAll('.grey-circle');
        },
        removeAvailableMovePositions: function () {
            this.getAvailableMovePositions().forEach(function (circle) {
                circle.remove();
            });
        },
        getGameId: function () {
            return gameId;
        },
        setGameId: function (Id) {
            gameId = Id;
        },
        getGameState: function () {
            return gameState;
        },
        setGameState: function (state) {
            gameState = state;
        },
        getGebruiker: function () {
            return gebruiker;
        },
        setGebruiker: function (user) {
            gebruiker = user;
        },
        getSpelers: function () {
            return spelers;
        },
        setSpelers: function (players) {
            spelers = players;
        },
        getCaptureMoves: function () {
            return captureMoves;
        },
        setCaptureMoves: function (moves) {
            captureMoves = moves;
        },
        isPlayersPiece: function (pieceState, gebruiker, spelers) {
            return (gebruiker === spelers[0] && (pieceState === 1 || pieceState === 3)) ||
                (gebruiker === spelers[1] && (pieceState === 2 || pieceState === 4));
        },
        getPossibleMoves: getPossibleMoves,
        movePiece: movePiece,
        GameStateArrayToGameState: GameStateArrayToGameState
    };
})();

function placePieces(gameState, spelers, gebruiker, Id, captureMoves) {
    CheckersModule.setGameId(Id);
    CheckersModule.setGameState(gameState);
    CheckersModule.setGameStateArray(gameState);
    CheckersModule.setSpelers(spelers);
    CheckersModule.setGebruiker(gebruiker);
    for (let i = 0; i < gameState.length; i++) {
        var squareState = parseInt(gameState.charAt(i));
        if (squareState === 0) continue;

        var squareDiv = document.querySelector('#shadowHost').shadowRoot.querySelector('#square' + i);

        var pieceDiv = document.createElement('div');
        pieceDiv.style.width = '75%';
        pieceDiv.style.height = '75%';
        pieceDiv.style.borderRadius = '50%';
        pieceDiv.style.position = 'relative';
        pieceDiv.style.left = '12.5%';
        pieceDiv.style.top = '12.5%';

        switch (squareState) {
            case 1:
                pieceDiv.style.backgroundColor = 'red';
                break;
            case 2:
                pieceDiv.style.backgroundColor = 'black';
                break;
            case 3:
                pieceDiv.style.backgroundColor = 'darkred';
                break;
            case 4:
                pieceDiv.style.backgroundColor = 'darkgray';
                break;
        }

        // Assign an id to the piece based on its value in the game state
        pieceDiv.id = 'piece' + squareState;

        squareDiv.appendChild(pieceDiv);

        if (CheckersModule.isPlayersPiece(squareState, gebruiker, spelers)) {
            (function (i, squareDiv) {
                var pieceDiv = squareDiv.firstChild;
                var clickHandler = function () {
                    // Check if the captureMoves array is not empty, null or undefined
                    if (captureMoves && captureMoves.length > 0) {
                        // If the piece is in the captureMoves array, show the moves
                        if (captureMoves.includes(pieceDiv)) {
                            CheckersModule.setSelectedPiece(squareDiv);
                            CheckersModule.getPossibleMoves(i, CheckersModule.getGameState(), CheckersModule.getGebruiker(), CheckersModule.getSpelers());
                        }
                    } else {
                        // If the captureMoves array is empty, null or undefined, use the old code
                        CheckersModule.setSelectedPiece(squareDiv);
                        CheckersModule.getPossibleMoves(i, CheckersModule.getGameState(), CheckersModule.getGebruiker(), CheckersModule.getSpelers());
                    }
                };
                pieceDiv.addEventListener('click', clickHandler);
                pieceDiv.clickHandler = clickHandler;
            })(i, squareDiv);
        }
    }
}

function checkForAdditionalCapture(squareDiv, capturedPrevious) {
    if (!capturedPrevious) {
        console.log("No capture move possible this turn");
        return;
    }
    var i = parseInt(squareDiv.id.replace('square', ''));
    var possibleMoves = CheckersModule.getPossibleMoves(i, CheckersModule.getGameState(), CheckersModule.getGebruiker(), CheckersModule.getSpelers());
    var captureMoves = []; // Array to store the capture moves

    for (var i = 0; i < possibleMoves.length; i++) {
        var move = possibleMoves[i];
        if (move.isCapture) {
            captureMoves.push(move); // Add the move to the captureMoves array
        }
    }

    // Set the captureMoves array in the CheckersModule
    CheckersModule.setCaptureMoves(captureMoves);

    if (captureMoves.length > 0) {
        console.log("Another capture is possible");
    } else {
        console.log("No capture moves left");
    }
}


function UpdateBoard(gameState, captureMoves) {
    // Iterate over each square and remove its child nodes
    for (let i = 0; i < 64; i++) { // Adjust the range according to your board size
        var square = document.querySelector('#shadowHost').shadowRoot.querySelector('#square' + i);
        while (square.firstChild) {
            square.removeChild(square.firstChild);
        }
    }

    // Place new pieces according to the gameState
    placePieces(gameState, CheckersModule.getSpelers(), CheckersModule.getGebruiker(), CheckersModule.getGameId(), captureMoves);
}
window.onload = function () {
    var spelersArray = JSON.parse('@Html.Raw(ViewBag.Spelers)');
    placePieces('@ViewBag.BordStand', spelersArray, '@ViewBag.Gebruiker');
}
