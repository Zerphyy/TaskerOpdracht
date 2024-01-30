function AddPlayerToGame(speler1, speler2, spel, event) {
    $.ajax({
        url: '/Dammen/AddPlayerToGame',
        method: 'POST',
        dataType: 'json',
        data: {
            GameData: {
                Speler1: speler1,
                Speler2: speler2,
                DamSpel: spel
            }
        },
        success: function (result) {
            if (result.success) {
                window.location.href = "/Dammen/Spel/" + result.id;
            } else {
                document.querySelector('#error__message').innerHTML = result.message;
                
            }
        },
        error: function (error) {
            console.error('Error:', error);
        }
    });
    event.preventDefault();
}
