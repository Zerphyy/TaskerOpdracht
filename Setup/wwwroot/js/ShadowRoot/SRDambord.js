// checkers board creator
var container = document.querySelector('#shadowHost').shadowRoot.querySelector('div');

var board = document.createElement('div');
board.style.width = '70%';
board.style.height = '100%';
board.style.display = 'grid';
board.style.gridTemplate = 'repeat(8, 1fr) / repeat(8, 1fr)';

for (var i = 0; i < 64; i++) {
    var square = document.createElement('div');
    square.style.width = '100%';
    square.style.height = '100%';
    square.style.backgroundColor = parseInt((i / 8) + i) % 2 == 0 ? '#B44C00' : '#F1E549';
    board.appendChild(square);
}

container.appendChild(board);