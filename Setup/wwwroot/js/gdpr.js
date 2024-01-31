class GDPR {
    constructor() {
        this.bindEvents();
        if (this.cookieStatus() !== 'accept') this.showGDPR();
    }

    bindEvents() {
        let buttonAccept = document.querySelector('.gdpr-consent__button--accept');
        buttonAccept.addEventListener('click', () => {
            console.log("accept is ingedrukt");
            this.cookieStatus('accept');
            this.hideGDPR();
        });


        //student uitwerking
        let buttonReject = document.querySelector('.gdpr-consent__button--reject');
        buttonReject.addEventListener('click', () => {
            console.log("reject is ingedrukt");
            this.cookieStatus('reject');
            this.hideGDPR();
        });
    }


    cookieStatus(status) {
        let today = new Date();
        let date = today.getDate() + '-' + (today.getMonth() + 1) + '-' + today.getFullYear();
        let time = today.getHours() + ":" + today.getMinutes() + ":" + today.getSeconds();
        const tijdNotatie = {
            datum: date,
            tijd: time
        }
        if (status) {
            localStorage.setItem('gdpr-consent-choice', status);
            localStorage.setItem("time-of-interaction", JSON.stringify(tijdNotatie));
        }


        //student uitwerking

        return localStorage.getItem('gdpr-consent-choice');
    }


    //student uitwerking


    hideGDPR() {
        document.querySelector(`.gdpr-consent`).classList.add('hide');
        document.querySelector(`.gdpr-consent`).classList.remove('show');
    }

    showGDPR() {
        document.querySelector(`.gdpr-consent`).classList.add('show');
    }
}
const gdpr = new GDPR();

