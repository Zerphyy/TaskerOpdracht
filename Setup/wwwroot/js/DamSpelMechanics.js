
var connection;
var isUpdating = false;
document.addEventListener('DOMContentLoaded', function () {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/gameHub")
        .build();
    connection.on("PlayerMoved", function (gameState, beurt) {
        console.log("hallo!");
        $.ajax({
            url: '/Dammen/UpdateBoardData',
            method: 'POST',
            dataType: 'json',
            data: {
                gameState: gameState,
                gameId: CheckersModule.getGameId(),
                beurt: beurt
            },
            success: function (result) {
                if (result.success) {
                    UpdateBoard(gameState, CheckersModule.getCaptureMoves(), beurt);
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
    var globalCaptureMoves = [];
    var lastCapturingPiece = null;
    var currentTurn = null;
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

    function addMoveListener(i, squareDiv, isCapture, enemyPos) {
        var circleDiv = createCircleDiv();
        circleDiv.addEventListener('click', function () {
            movePiece(i, squareDiv, isCapture, enemyPos);
        });
        squareDiv.appendChild(circleDiv);
    }

    //index contains the row and col of the new square, i is the index of the current square.
    //enemyPos contains the row and col of the enemy position when a capture move gets made
    function checkMove(index, i, isCapture, enemyPos = null) {
        if (index !== null && index.row >= 0 && index.row < 8 && index.col >= 0 && index.col < 8 && gameStateArray[index.row][index.col] === 0) {
            var squareDiv = CheckersModule.getShadowRoot().querySelector('#square' + (index.row * 8 + index.col));
            addMoveListener(i, squareDiv, isCapture, enemyPos);
            return { "squareDiv": squareDiv, "isCapture": isCapture };
        }
    }

    function getPossibleNormalMoves(i, gameState, gebruiker, spelers) {
        if (CheckersModule.getCurrentTurn() != gebruiker) {
            console.log("its not your turn!    " + CheckersModule.getCurrentTurn());
            return;
        }
        CheckersModule.setGlobalCaptureMoves([]);
        var normalMoves = [];
        console.log(CheckersModule.getLastCapturingPiece());
        var row = Math.floor(i / 8);
        var col = i % 8;

        var leftSquareIndex, rightSquareIndex;
        var squareState = parseInt(gameState.charAt(i));
        if (gebruiker === spelers[0]) { // Player 1 (top) moves down
            leftSquareIndex = (col > 0 && row < 7) ? { row: row + 1, col: col - 1 } : null;
            rightSquareIndex = (col < 7 && row < 7) ? { row: row + 1, col: col + 1 } : null;
        } else { // Player 2 (bottom) moves up
            leftSquareIndex = (col > 0 && row > 0) ? { row: row - 1, col: col - 1 } : null;
            rightSquareIndex = (col < 7 && row > 0) ? { row: row - 1, col: col + 1 } : null;
        }

        CheckersModule.removeAvailableMovePositions();

        // Check if the selected piece is the last capturing piece
        if (CheckersModule.getLastCapturingPiece() !== null && CheckersModule.getLastCapturingPiece() !== i) {
            console.log("Wrong piece pressed. You must select the last capturing piece.");
            return []; // Prevent any moves if the wrong piece is selected
        }

        // Check for capture moves first
        var captureMoves = checkForPossibleCaptures(i, gebruiker, spelers);

        // If there are capture moves available, only allow them
        if (captureMoves.length > 0) {
            captureMoves.forEach(function (index) {
                ind = { row: index.row, col: index.col };
                enemyPos = { row: index.enemyRow, col: index.enemyCol };
                var move = checkMove(ind, i, true, enemyPos); // isCapture = true
                if (move) {
                    CheckersModule.setGlobalCaptureMoves([...CheckersModule.getGlobalCaptureMoves(), move]); // Save to global array
                }
            });
            return CheckersModule.getGlobalCaptureMoves(); // Return only capture moves
        }

        // Otherwise, check for normal moves only if the last capturing piece is null
        [leftSquareIndex, rightSquareIndex].forEach(function (index) {
            if (index !== null && index.row >= 0 && index.col >= 0 && CheckersModule.getGameStateArray()[index.row][index.col] === 0) {
                var move = checkMove(index, i, false); // isCapture = false
                if (move) {
                    normalMoves.push(move);
                }
            }
        });

        return normalMoves; // Return normal moves if no captures
    }
    function getPossiblePromotedMoves(i, gameState) {
        if (CheckersModule.getCurrentTurn() != gebruiker) {
            console.log("its not your turn!    " + CheckersModule.getCurrentTurn());
            return;
        }
        CheckersModule.removeAvailableMovePositions();
        // Run the recursive diagonal checker for promoted pieces
        checkDiagonalMoves(i, 1, 1, gameState); // Right-down
        checkDiagonalMoves(i, 1, -1, gameState); // Right-up
        checkDiagonalMoves(i, -1, 1, gameState); // Left-down
        checkDiagonalMoves(i, -1, -1, gameState); // Left-up
    }

    //i contains the current piece location
    //square div contains the new location
    function movePiece(i, squareDiv, isCapture, enemyPos) {
        if (isUpdating) {
            console.log('Game state is updating, please wait...');
            return;
        }

        isUpdating = true; // Set the flag to true when starting the update
        CheckersModule.removeAvailableMovePositions();

        let square = CheckersModule.getSelectedPiece();
        let piece = square.firstChild;
        let pieceState = CheckersModule.getGameStateArray()[Math.floor(i / 8)][i % 8];
        if (piece.id === 'piece3' && pieceState == 1) {
            pieceState = 3;
        } else if (piece.id === 'piece4' && pieceState == 2) {
            pieceState = 4;
        }

        // Check if lastCapturingPiece is set and prevent movement of any piece other than the last capturing piece
        if (CheckersModule.getLastCapturingPiece() !== null) {
            if (CheckersModule.getLastCapturingPiece() !== i) {
                console.log("Only the last capturing piece can be moved.");
                isUpdating = false;
                return;
            }
        }

        square.removeChild(piece);
        squareDiv.appendChild(piece);
        piece.removeEventListener('click', piece.clickHandler);

        i = Array.from(squareDiv.parentNode.children).indexOf(squareDiv);

        CheckersModule.getGameStateArray()[Math.floor(square.id.slice(6) / 8)][square.id.slice(6) % 8] = 0;
        CheckersModule.getGameStateArray()[Math.floor(i / 8)][i % 8] = pieceState;

        // Handle capture logic
        if (isCapture) {
            var enemyRow = enemyPos.row;
            var enemyCol = enemyPos.col;
            CheckersModule.getGameStateArray()[enemyRow][enemyCol] = 0;
            var enemySquare = document.querySelector('#shadowHost').shadowRoot.querySelector('#square' + (enemyRow * 8 + enemyCol));
            enemySquare.removeChild(enemySquare.firstChild);

            CheckersModule.setLastCapturingPiece(i); // Update lastCapturingPiece on a successful capture
            CheckersModule.setGlobalCaptureMoves([...CheckersModule.getGlobalCaptureMoves(), { squareDiv, isCapture }]); // Save capturing move for potential next turn
        } else {
            CheckersModule.setLastCapturingPiece(null); // Reset if no capture
        }

        CheckersModule.setGameStateArray(CheckersModule.getGameStateArray().flat().join(''));

        // Call check for additional capture after the first capture
        if (isCapture) {
            //call the new checkForAdditionalPromotedCapture function here in case the piece is promoted, else call the original checkForAdditionalCapture
            if (pieceState == 3 || pieceState == 4) {
                checkForAdditionalPromotedCaptures(squareDiv, CheckersModule.getGebruiker(), CheckersModule.getSpelers(), isCapture);
            }
            checkForAdditionalCapture(squareDiv, isCapture);
        }
        else {
            CheckersModule.setGlobalCaptureMoves([]); // Reset global capture moves after the first capture
        }

        // If additional capture exists, update moves accordingly
        if (CheckersModule.getGlobalCaptureMoves().length > 0) {
            // Removed adding grey circles
            //var possibleMoves = CheckersModule.getPossibleNormalMoves(i, CheckersModule.getGameState(), CheckersModule.getGebruiker(), CheckersModule.getSpelers());
            // Logic to add moves without grey circles

            // Update the game state for the database here after processing additional captures
            CheckersModule.setGameState(CheckersModule.GameStateArrayToGameState());
            connection.invoke("NotifyPlayerMoved", CheckersModule.getGameState(), CheckersModule.getCurrentTurn()).catch(function (err) {
                console.error(err.toString());
            });
        } else {
            // End the turn if no additional captures
            CheckersModule.setSelectedPiece(piece);
            CheckersModule.setGameState(CheckersModule.GameStateArrayToGameState());
            CheckersModule.setCurrentTurn(CheckersModule.getCurrentTurn() === CheckersModule.getSpelers()[0] ? CheckersModule.getSpelers()[1] : CheckersModule.getSpelers()[0]);
            connection.invoke("NotifyPlayerMoved", CheckersModule.getGameState(), CheckersModule.getCurrentTurn()).catch(function (err) {
                console.error(err.toString());
            });
        }
    }

    function checkForPossiblePromotedCapture(ogPos, enemyPos, dirX, dirY, gameState) {
        var gebruiker = CheckersModule.getGebruiker();
        var spelers = CheckersModule.getSpelers();
        var row = Math.floor(ogPos / 8);
        var col = ogPos % 8;
        var enemyRow = Math.floor(enemyPos / 8);
        var enemyCol = enemyPos % 8;
        var jumpRow = enemyRow + dirX;
        var jumpCol = enemyCol + dirY;
        var jumpPos = CheckersModule.getGameStateArray()[jumpRow][jumpCol];
        if ((gebruiker === spelers[0] && (gameState.charAt(enemyPos) == 2 || gameState.charAt(enemyPos) == 4) ||
            (gebruiker === spelers[1] && (gameState.charAt(enemyPos) == 1 || gameState.charAt(enemyPos) == 3))) && jumpPos === 0) {
            //enemy piece has been found, check if the next square is empty, and add a listener
            CheckersModule.checkMove({ row: jumpRow, col: jumpCol }, ogPos, true, { row: enemyRow, col: enemyCol });
        } else {
            //friendly piece encountered, or no free space behind the detected piece, no further action taken
            return;
        }
    }

    function checkForPossibleCaptures(i, gebruiker, spelers) {
        var possibleCaptures = [];
        var gameStateArray = CheckersModule.getGameStateArray();
        var row = Math.floor(i / 8);
        var col = i % 8;

        var directions = [];

        // Determine allowed capture directions based on the player
        if (gebruiker === spelers[0]) { // Player 1 (top) can capture downward
            directions = [[1, -1], [1, 1]];
        } else { // Player 2 (bottom) can capture upward
            directions = [[-1, -1], [-1, 1]];
        }

        directions.forEach(function (direction) {
            var newRow = row + direction[0];
            var newCol = col + direction[1];
            var jumpRow = row + 2 * direction[0];
            var jumpCol = col + 2 * direction[1];

            // Ensure the move is within board bounds (0 to 7) and that the piece being jumped is an enemy
            if (
                newRow >= 0 && newRow < 8 &&
                newCol >= 0 && newCol < 8 &&
                jumpRow >= 0 && jumpRow < 8 &&
                jumpCol >= 0 && jumpCol < 8 &&
                gameStateArray[newRow][newCol] !== 0 && // There is a piece in between
                ((gebruiker === spelers[0] && gameStateArray[newRow][newCol] === 2) || // Player 1 can only capture black pieces
                    (gebruiker === spelers[1] && gameStateArray[newRow][newCol] === 1)) && // Player 2 can only capture red pieces
                gameStateArray[jumpRow][jumpCol] === 0  // Ensure the destination is empty
            ) {
                var squareDiv = CheckersModule.getShadowRoot().querySelector('#square' + (jumpRow * 8 + jumpCol));
                possibleCaptures.push({ row: jumpRow, col: jumpCol, enemyRow: newRow, enemyCol: newCol });
            }
        });

        return possibleCaptures;
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
        getCurrentTurn: function () {
            return currentTurn;
        },
        setCurrentTurn: function (ct) {
            if (ct === null) {
                currentTurn = CheckersModule.getSpelers()[0];
            } else {
                currentTurn = ct;
            }
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
        setGlobalCaptureMoves: function (gcm) {
            globalCaptureMoves = gcm;
        },
        getGlobalCaptureMoves: function () {
            return globalCaptureMoves;
        },
        setLastCapturingPiece: function (lcp) {
            lastCapturingPiece = lcp;
        },
        getLastCapturingPiece: function () {
            return lastCapturingPiece;
        },
        isPlayersPiece: function (pieceState, gebruiker, spelers) {
            return (gebruiker === spelers[0] && (pieceState === 1 || pieceState === 3)) ||
                (gebruiker === spelers[1] && (pieceState === 2 || pieceState === 4));
        },
        getPossibleNormalMoves: getPossibleNormalMoves,
        getPossiblePromotedMoves: getPossiblePromotedMoves,
        movePiece: movePiece,
        GameStateArrayToGameState: GameStateArrayToGameState,
        checkForPossibleCaptures: checkForPossibleCaptures,
        checkForPossiblePromotedCapture: checkForPossiblePromotedCapture,
        checkMove: checkMove
    };
})();

function placePieces(gameState, spelers, gebruiker, Id, captureMoves, ct) {
    CheckersModule.setGameId(Id);
    CheckersModule.setGameState(gameState);
    CheckersModule.setGameStateArray(gameState);
    CheckersModule.setSpelers(spelers);
    CheckersModule.setGebruiker(gebruiker);
    CheckersModule.setCurrentTurn(ct);
    for (let i = 0; i < gameState.length; i++) {
        var squareState = parseInt(gameState.charAt(i));
        if (squareState === 0) continue;
        if (squareState === 1 && i < gameState.length && i >= gameState.length - 8) {
            squareState = 3;
        }
        if (squareState === 2 && i >= 0 && i < 8) {
            squareState = 4;
        }
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
                    CheckersModule.setSelectedPiece(squareDiv);
                    if (pieceDiv.id === 'piece3' || pieceDiv.id === 'piece4') {
                        CheckersModule.getPossiblePromotedMoves(i, gameState);
                    } else {
                        // Normal piece move handling
                        if (captureMoves && captureMoves.length > 0 && captureMoves.includes(pieceDiv)) {
                            CheckersModule.getPossibleNormalMoves(i, CheckersModule.getGameState(), CheckersModule.getGebruiker(), CheckersModule.getSpelers());
                        } else {
                            CheckersModule.getPossibleNormalMoves(i, CheckersModule.getGameState(), CheckersModule.getGebruiker(), CheckersModule.getSpelers());
                        }
                    }
                };
                pieceDiv.addEventListener('click', clickHandler);
                pieceDiv.clickHandler = clickHandler;
            })(i, squareDiv);
        }
    }
    //check if a player captured all enemy pieces
    //if yes, end game
    piecesLeftOnBoard();
}

function checkDiagonalMoves(originalPosition, horizontal, vertical, gameState) {

    let row = Math.floor(originalPosition / 8);
    let col = originalPosition % 8;
    let nextRow = row + vertical;
    let nextCol = col + horizontal;

    while (nextRow >= 0 && nextRow < 8 && nextCol >= 0 && nextCol < 8) {
        let nextPosition = nextRow * 8 + nextCol;
        let squareState = parseInt(gameState.charAt(nextPosition));

        if (squareState === 0) {
            // Call checkMove for legitimate empty squares
            let index = { row: nextRow, col: nextCol };
            CheckersModule.checkMove(index, originalPosition, false); // false since it's not a capture
        } else {
            CheckersModule.checkForPossiblePromotedCapture(originalPosition, nextPosition, vertical, horizontal, gameState);
            break;
        }

        // Continue checking in the same diagonal direction
        nextRow += vertical;
        nextCol += horizontal;
    }
}

function checkForAdditionalPromotedCaptures(i, speler, spelers, capturedPrevious) {
    if (!capturedPrevious) {
        console.log("No capture move possible this turn");
        return;
    }
    i = parseInt(i.id.replace('square', ''));
    var row = Math.floor(i / 8);
    var col = i % 8;
    var captureMoves = [];

    // Directions for diagonals (row and col increments for diagonal directions)
    var directions = [
        { rowDir: 1, colDir: 1 },  // Down-right
        { rowDir: 1, colDir: -1 }, // Down-left
        { rowDir: -1, colDir: 1 }, // Up-right
        { rowDir: -1, colDir: -1 } // Up-left
    ];

    directions.forEach(function (direction) {
        if (captureMoves.length > 0) {
            CheckersModule.setGlobalCaptureMoves(captureMoves);
            return;
        }
        var currentRow = row + direction.rowDir;
        var currentCol = col + direction.colDir;
        var capturableEnemy = null;

        // Keep moving diagonally until out of bounds or encountering a piece
        while (currentRow >= 0 && currentRow < 8 && currentCol >= 0 && currentCol < 8) {
            var squareState = CheckersModule.getGameStateArray()[currentRow][currentCol];

            if (squareState === 0) {
                // Empty square, keep going
                currentRow += direction.rowDir;
                currentCol += direction.colDir;
                continue;
            } else if (capturableEnemy === null && speler === spelers[0] && (squareState === 2 || squareState === 4) || speler === spelers[1] && (squareState === 1 || squareState === 3)) {
                // Found an enemy piece, check if we can capture it
                capturableEnemy = { row: currentRow, col: currentCol };
            } else if (capturableEnemy !== null && CheckersModule.getGameStateArray()[currentRow + direction.rowDir][currentCol + direction.colDir] === 0) {
                // Empty square after the enemy piece, valid capture move
                captureMoves.push({
                    captureRow: capturableEnemy.row,
                    captureCol: capturableEnemy.col,
                    moveRow: currentRow,
                    moveCol: currentCol
                });

                break;
            } else {
                // Encountered a friendly piece or another blocking piece, stop searching this direction
                break;
            }
        }
    });
    CheckersModule.setGlobalCaptureMoves(captureMoves);
}

function checkForAdditionalCapture(squareDiv, capturedPrevious) {
    if (!capturedPrevious) {
        console.log("No capture move possible this turn");
        return;
    }

    var shadowRoot = document.querySelector('#shadowHost').shadowRoot;

    var possibleMoves = CheckersModule.getPossibleNormalMoves(
        parseInt(squareDiv.id.replace('square', '')),
        CheckersModule.getGameState(),
        CheckersModule.getGebruiker(),
        CheckersModule.getSpelers()
    );

    var canCaptureAgain = false; // Flag to check if another capture is possible

    // Iterate through possible moves
    for (var j = 0; j < possibleMoves.length; j++) {
        var move = possibleMoves[j];
        var targetSquareId = move.squareDiv.id; // Get the target square ID
        var targetSquare = shadowRoot.querySelector('#' + targetSquareId); // Get the target square element from shadowRoot

        if (targetSquare) {
            var currentSquareIndex = parseInt(squareDiv.id.replace('square', ''));
            var targetSquareIndex = parseInt(targetSquareId.replace('square', ''));

            // Check if this is a 1-step non-capture move (no jumping involved)
            if (Math.abs(currentSquareIndex - targetSquareIndex) === 9 || Math.abs(currentSquareIndex - targetSquareIndex) === 7) {
                // Non-capture move (just a 1-step move)
                var pieceElement = targetSquare.querySelector('div'); // Check if there's a piece in the target square

                if (pieceElement && pieceElement.className === 'grey-circle') {
                    canCaptureAgain = false;
                }
            } else {
                // Check for capture moves (jumps)
                var middleSquareIndex = (currentSquareIndex + targetSquareIndex) / 2;
                var middleSquare = shadowRoot.querySelector('#square' + middleSquareIndex); // Get the middle square

                if (middleSquare) {
                    // Check if the middle square has a piece inside it (the piece being jumped over)
                    var pieceElement = middleSquare.querySelector('div'); // Get the child div (the piece element)

                    if (pieceElement) {
                        // Check the ID of the piece element
                        var pieceId = pieceElement.id; // Should be 'piece1' (red) or 'piece2' (black)

                        // Check if the piece is capturable (an opponent's piece)
                        if (CheckersModule.getGebruiker() === CheckersModule.getSpelers()[0] && pieceId === 'piece2' ||
                            CheckersModule.getGebruiker() === CheckersModule.getSpelers()[1] && pieceId === 'piece1') {
                            canCaptureAgain = true; // A valid capture move is possible
                        } else {
                            canCaptureAgain = false;
                        }
                    } else {
                        canCaptureAgain = false;
                    }
                }
            }
        }
    }

    if (canCaptureAgain) {
        console.log("Another capture is possible");
        // Allow the player to continue making captures
    } else {
        console.log("No capture moves left");
        // Clear lastCapturingPiece and globalCaptureArray
        CheckersModule.setLastCapturingPiece(null); // Clear the lastCapturingPiece
        CheckersModule.setGlobalCaptureMoves([]); // Clear the globalCaptureArray
    }
}
function UpdateBoard(gameState, captureMoves, currentTurn) {
    // Iterate over each square and remove its child nodes
    for (let i = 0; i < 64; i++) { // Adjust the range according to your board size
        var square = document.querySelector('#shadowHost').shadowRoot.querySelector('#square' + i);
        while (square.firstChild) {
            square.removeChild(square.firstChild);
        }
    }

    // Place new pieces according to the gameState
    placePieces(gameState, CheckersModule.getSpelers(), CheckersModule.getGebruiker(), CheckersModule.getGameId(), captureMoves, currentTurn);
}
function piecesLeftOnBoard() {
    var gameState = CheckersModule.getGameState();

    // Check if both players still have pieces
    if ((gameState.includes('1') || gameState.includes('3')) && (gameState.includes('2') || gameState.includes('4'))) {
        return; // No one has won yet
    }
    // Check if Speler 0 has pieces but Speler 1 doesn't
    else if ((gameState.includes('1') || gameState.includes('3')) && !(gameState.includes('2') || gameState.includes('4'))) {
        alert('Speler 0 heeft gewonnen!');
        window.location.href = '/Home/'; // Redirect after the alert
    }
    // Check if Speler 1 has pieces but Speler 0 doesn't
    else if (!(gameState.includes('1') || gameState.includes('3')) && (gameState.includes('2') || gameState.includes('4'))) {
        alert('Speler 1 heeft gewonnen!');
        window.location.href = '/Home/'; // Redirect after the alert
    }
}
window.onload = function () {
    var spelersArray = JSON.parse('@Html.Raw(ViewBag.Spelers)');
    placePieces('@ViewBag.BordStand', spelersArray, '@ViewBag.Gebruiker');
}