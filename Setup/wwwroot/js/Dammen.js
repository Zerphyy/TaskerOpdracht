document.addEventListener('DOMContentLoaded', function () {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/gameHub")
        .build();

    connection.on("GameListChanged", function () {
        GetGameList();
    });

    connection.start().then(function () {
        console.log("SignalR Connected");
    }).catch(function (err) {
        console.error(err.toString());
    });
});

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

function RemoveGame(spel) {
    $.ajax({
        url: '/Dammen/Delete',
        method: 'POST',
        dataType: 'json',
        data: {
            spel: spel
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
}
function CreateTablePartials(data) {
    document.querySelector('#game__lijst').innerHTML = '';
    document.querySelector('#eigen__game__lijst').innerHTML = '';
    for (var spel of data.Spellen) {
        var genericRij = CreateTablePartialData(spel, data.Spelers, data.Gebruiker);
        var eigenRij = CreateTablePartialData(spel, data.Spelers, data.Gebruiker);
        document.querySelector('#game__lijst').appendChild(genericRij);
        if (spel.creator === data.Gebruiker) {
            var verwijderentd = document.createElement('td');
            var verwijderen = document.createElement('button');
            verwijderen.onclick = (function (currentSpel) {
                return function () {
                    RemoveGame(currentSpel);
                };
            })(spel);
            verwijderen.innerHTML = 'verwijderen';
            verwijderentd.appendChild(verwijderen);
            eigenRij.appendChild(verwijderentd);
            document.querySelector('#eigen__game__lijst').appendChild(eigenRij);
        }
    }
}
function CreateTablePartialData(spel, spelers, gebruiker) {
    var rij = document.createElement('tr');

    var spelNaam = document.createElement('td');
    var maker = document.createElement('td');
    var deelnemer = document.createElement('td');
    var bekijken = document.createElement('a');
    var bekijkentd = document.createElement('td');
    var meedoen = document.createElement('button');
    var meedoentd = document.createElement('td');

    spelNaam.innerHTML = spel.spelNaam;


    maker.innerHTML = '...';
    deelnemer.innerHTML = '...';
    for (var speler of spelers) {
        if (speler.email === spel.creator) {
            maker.innerHTML = speler.naam;
        }
        if (speler.email === spel.deelnemer) {
            deelnemer.innerHTML = speler.naam;
        }
    }

    bekijken.href = '/Dammen/Spel/' + spel.id;
    bekijken.innerHTML = 'Bekijken';
    bekijkentd.appendChild(bekijken);

    meedoen.onclick = (function (currentSpel) {
        return function () {
            AddPlayerToGame(currentSpel.creator, gebruiker, currentSpel);
        };
    })(spel);
    meedoen.innerHTML = 'Meedoen';
    meedoentd.appendChild(meedoen);



    rij.appendChild(spelNaam);
    rij.appendChild(maker);
    rij.appendChild(deelnemer);
    rij.appendChild(bekijkentd);
    rij.appendChild(meedoentd);
    return rij;
}

function GetGameList() {
    $.ajax({
        url: 'Dammen/GetGameLijst',
        method: 'GET',
        dataType: 'json',
        success: function (result) {
            CreateTablePartials(result);
        },
        error: function (error) {
            console.error(error);
        }
    })
}

window.onload = function () {
    GetGameList();
};
