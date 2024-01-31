var connection;
document.addEventListener('DOMContentLoaded', function () {

    connection = new signalR.HubConnectionBuilder()
        .withUrl("/gameHub")
        .build();
        
    connection.on("GameChanged", function () {
        location.reload();
    });

    connection.start().then(function () {
        console.log("SignalR Connected");
    });
});
function reloadPage() {
    connection.invoke("SendMessage").catch(function (err) {
        console.error(err.toString());
    });
}

function AddPlayerToGame(speler1, speler2, spel) {
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
                connection.invoke("NotifyGameChanged").catch(function (err) {
                    console.error(err.toString());
                });
            } else {
                document.querySelector('#error__message').innerHTML = result.message;
            }
        },
        error: function (error) {
            console.error('Error:', error);
        }
    });
}

function RemoveGame(id, event) {
    $.ajax({
        url: '/Dammen/Delete',
        method: 'POST',
        dataType: 'json',
        data: {
            id: id
        },
        success: function (result) {
            if (result.success) {
                connection.invoke("NotifyGameChanged").catch(function (err) {
                    console.error(err.toString());
                });
            } else {
                document.querySelector('#error__message').innerHTML = result.message;
            }
        },
        error: function (error) {
            console.error('Error:', error);
        }
    })
    event.preventDefault();
}