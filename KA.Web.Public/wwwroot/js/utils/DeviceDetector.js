export class DeviceDetector {
    static isMobile() {
        return /iPhone|iPod|Android|Windows CE|BlackBerry|Symbian|Windows Phone|webOS|Opera Mini|Opera Mobi|POLARIS|IEMobile|lgtelecom|nokia|SonyEricsson|LG|SAMSUNG|Samsung/i.test(
            navigator.userAgent
        );
    }

    static isMacintosh() {
        return (
            ['iPad Simulator', 'iPhone Simulator', 'iPod Simulator', 'iPad', 'iPhone', 'iPod'].includes(navigator.platform)
            // iPad on iOS 13 detection
            || navigator.userAgent.includes('Mac')
        );
    }
}
