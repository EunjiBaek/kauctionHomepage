export class Localizer {
    static getLanguage() {
        return document.querySelector('html[lang]').getAttribute('lang');
    }
}
