import { Localizer } from '../../utils/Localizer.js';

export class DatePicker {
    constructor(calenderId, years) {
        this.$calender = document.querySelector(`#${calenderId}`);
        this.$input = this.$calender.querySelector('input');
        this.lang = Localizer.getLanguage();
        this.years = years;
        this.init();
    }
    
    init = () => {
        this.#setYearRange(this.years);
    }

    getSearchDateRange = () => {
        const dateRange = this.$input.value;
        const separator = this.lang === 'ko-KR' ? '~' : 'to';
        const [startDate, endDate] = dateRange.split(separator);
        return [dayjs(startDate).toISOString(), dayjs(endDate).endOf('day').toISOString()];
    };

    #setYearRange(years) {
        this.$calender.flatpickr({
            mode: 'range',
            dateFormat: 'Y-m-d',
            wrap: true,
            locale: this.lang === 'ko-KR' ? 'ko' : 'en',
            defaultDate: [dayjs().subtract(years, 'year').format('YYYY-MM-DD'), dayjs().format('YYYY-MM-DD')],
        });
    };
}
