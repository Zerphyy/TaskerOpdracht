let currentUserRole = "";
function setCurrentUserRole(user) {
    GetUserRole(user);
    GetUserList();
}
function CreateTablePartials(data) {
    document.querySelector('#user__lijst').innerHTML = '';
    for (var user of data) {
        var genericRij = CreateTablePartialData(user);
        document.querySelector('#user__lijst').appendChild(genericRij);
    }
}
function CreateTablePartialData(user) {
    var rij = document.createElement('tr');

    var username = document.createElement('td');
    var email = document.createElement('td');
    var spellenGespeeld = document.createElement('td');
    var spellenGewonnen = document.createElement('td');
    var spellenVerloren = document.createElement('td');
    var winLossRatio = document.createElement('td');
    var rol = document.createElement('td');
    var promoverenCell = document.createElement('td');
    var verwijderenCell = document.createElement('td');

    var stats = GetUserStats(user);
    username.innerHTML = user.naam;
    email.innerHTML = user.email;
    spellenGespeeld.innerHTML = stats != undefined && stats.aantalSpellen != undefined ? stats.aantalSpellen : '0';
    spellenGewonnen.innerHTML = stats != undefined && stats.aantalGewonnen != undefined ? stats.aantalGewonnen : '0';
    spellenVerloren.innerHTML = stats != undefined && stats.aantalVerloren != undefined ? stats.aantalVerloren : '0';
    winLossRatio.innerHTML = stats != undefined && stats.aantalVerloren != undefined && stats.aantalVerloren != 0 ? stats.winLossRatio : '0';
    rol.innerHTML = user.rol;

    rij.appendChild(username);
    rij.appendChild(email);
    rij.appendChild(spellenGespeeld);
    rij.appendChild(spellenGewonnen);
    rij.appendChild(spellenVerloren);
    rij.appendChild(winLossRatio);
    rij.appendChild(rol);

    if (currentUserRole == 'Moderator') {
        if (user.rol != 'Moderator') {
            var promoveren = document.createElement('button');
            promoveren.innerHTML = 'Promoveren';
            promoveren.onclick = function () {
                console.log(user.email);
                PromoteUser(user.email);
            };
            promoverenCell.appendChild(promoveren);
            rij.appendChild(promoverenCell);
            var verwijderen = document.createElement('button');
            verwijderen.innerHTML = 'Verwijderen';
            verwijderen.onclick = function () {
                RemoveUser(user.email);
            };
            verwijderenCell.appendChild(verwijderen);
            rij.appendChild(verwijderenCell);
        }
    } else if (currentUserRole == 'Admin') {
        if (user.rol != 'Admin') {
            if (user.rol != 'Moderator') {
                var promoveren = document.createElement('button');
                promoveren.innerHTML = 'Promoveren';
                promoveren.onclick = function () {
                    console.log(user.email);
                    PromoteUser(user.email);
                };
                promoverenCell.appendChild(promoveren);
                rij.appendChild(promoverenCell);
            } else {
                rij.appendChild(promoverenCell);
            }
            var verwijderen = document.createElement('button');
            verwijderen.innerHTML = 'Verwijderen';
            verwijderen.onclick = function () {
                RemoveUser(user.email);
            };
            verwijderenCell.appendChild(verwijderen);
            rij.appendChild(verwijderenCell);
        }
    } else {
        rij.appendChild(promoverenCell);
        rij.appendChild(verwijderenCell);
    }
    return rij;
}

function GetUserList() {
    $.ajax({
        url: 'Management/GetUserLijst',
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
function GetUserStats(user) {
    $.ajax({
        url: 'Management/GetUserStats',
        method: 'GET',
        dataType: 'json',
        success: function (result) {
            return result;
        },
        error: function (error) {
            console.error(error);
        }
    })
}
function GetUserRole(user) {
    $.ajax({
        url: 'Management/GetUserRole',
        method: 'GET',
        dataType: 'json',
        data: {
            user: user
        },
        success: function (result) {
            currentUserRole = result;
        },
        error: function (error) {
            console.error(error);
        }
    })
}
function PromoteUser(user) {
    $.ajax({
        url: 'Management/PromoteUser',
        method: 'Post',
        dataType: 'json',
        data: {
            userId: user
        },
        success: function (result) {
            location.reload();
        },
        error: function (error) {
            console.error(error);
        }
    })
}
function RemoveUser(user) {
    $.ajax({
        url: 'Management/RemoveUser',
        method: 'POST',
        dataType: 'json',
        data: {
            userId: user
        },
        success: function (result) {
            if (result.success) {
                location.reload();
            } else {
                alert('Error: ' + result.message);
            }
        },
        error: function (error) {
            console.error(error);
        }
    });
}