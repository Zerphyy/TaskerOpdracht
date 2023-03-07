class testing {
    test() {
        console.log("hello");
    }
}

document.addEventListener('DOMContentLoaded', () => {
    const createGameButton = document.querySelector('#create-game-button');
    const t = new testing();
    createGameButton.addEventListener('click', t.test);
});

//use this class to see how razor -> javascript works. check out the Create Game button in Dammen->Index