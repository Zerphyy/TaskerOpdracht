var shadow = document.querySelector('#shadowHost').attachShadow({ mode: 'open' });

var container = document.createElement('div');
container.style.display = 'flex';
container.style.width = '90vw';
container.style.height = '60vw';

shadow.appendChild(container);
