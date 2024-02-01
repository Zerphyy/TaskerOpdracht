var connection;
document.addEventListener('DOMContentLoaded', function () {

    connection = new signalR.HubConnectionBuilder()
        .withUrl("/gameHub")
        .build();

    connection.on("GameChanged", function () {
        GetGameList();
    });

    connection.start().then(function () {
        console.log("SignalR Connected");
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
    event.preventDefault();
}
function CreateTablePartials(data) {
        CreateTablePartialGeneric(data);
        CreateTablePartialPersonal(data);
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

//create table for all games
function CreateTablePartialGeneric(data) {
    genericTable = document.querySelector('#game__lijst');
    genericTable.innerHTML = '';
    for (var spel of data.Spellen) {
        var row = document.createElement('tr');

        var spelNaam = document.createElement('td');
        spelNaam.innerHTML = spel.spelNaam;

        var creator = document.createElement('td');
        for (var speler of data.Spelers) {
            if (spel.creator == speler.email) {
                creator.innerHTML = speler.naam;
            }
        }
        

        var deelnemer = document.createElement('td');
        deelnemer.innerHTML = '...';
        for (var speler of data.Spelers) {
            if (spel.deelnemer == speler.email) {
                deelnemer.innerHTML = speler.naam;
            }
        }

        var bekijkentd = document.createElement('td');
        var bekijken = document.createElement('a');
        bekijken.setAttribute('asp-action', 'Spel');
        bekijken.setAttribute('asp-route-id', spel.id);
        bekijken.innerHTML = 'Bekijken';
        bekijkentd.appendChild(bekijken);

        var meedoentd = document.createElement('td');
        var meedoen = document.createElement('button');
        meedoen.innerHTML = 'Meedoen';
        meedoen.onclick = function () {
            AddPlayerToGame(spel.creator, data.Gebruiker, spel);
        };
        meedoentd.appendChild(meedoen);

        row.appendChild(spelNaam);
        row.appendChild(creator);
        row.appendChild(deelnemer);
        row.appendChild(bekijkentd);
        row.appendChild(meedoentd);

        genericTable.appendChild(row);
    }
}
//create table for all games you made
function CreateTablePartialPersonal(data) {
    var personalTable = document.querySelector('#eigen__game__lijst');
    personalTable.innerHTML = '';
    for (var spel of data.Spellen) {
        if (spel.creator == data.Gebruiker) {
            var row = document.createElement('tr');

            var spelNaam = document.createElement('td');
            spelNaam.innerHTML = spel.spelNaam;

            var creator = document.createElement('td');
            for (var speler of data.Spelers) {
                if (spel.creator == speler.email) {
                    creator.innerHTML = speler.naam;
                }
            }

            var deelnemer = document.createElement('td');
            deelnemer.innerHTML = '...';
            for (var speler of data.Spelers) {
                if (spel.deelnemer == speler.email) {
                    deelnemer.innerHTML = speler.naam;
                }
            }

            var bekijkentd = document.createElement('td');
            var bekijken = document.createElement('a');
            bekijken.setAttribute('asp-action', 'Spel');
            bekijken.setAttribute('asp-route-id', spel.id);
            bekijken.innerHTML = 'Bekijken';
            bekijkentd.appendChild(bekijken);

            var meedoentd = document.createElement('td');
            var meedoen = document.createElement('button');
            meedoen.innerHTML = 'Meedoen';
            meedoen.onclick = function () {
                AddPlayerToGame(spel.creator, data.Gebruiker, spel);
            };
            meedoentd.appendChild(meedoen);

            var verwijdertd = document.createElement('td');
            var verwijder = document.createElement('button');
            verwijder.innerHTML = 'Verwijderen';
            verwijder.onclick = function () {
                RemoveGame(spel);
            }
            verwijdertd.appendChild(verwijder);

            row.appendChild(spelNaam);
            row.appendChild(creator);
            row.appendChild(deelnemer);
            row.appendChild(bekijkentd);
            row.appendChild(meedoentd);
            row.appendChild(verwijdertd);

            personalTable.appendChild(row);
        }
    }
}
//CALL
//PAKT VIA C# FUNCTION ALLE VIEWBAG DATA
//RETURNT DEZE NAAR EEN ANDERE JS FUNCTION
//DEZE JS FUNCTION REPLACED DE TBODY ELEMENTEN