export class RouteValidator {
    static lastSegmentStartsWith(str) {
        return window.location.pathname.split('/').pop().startsWith(str);
    }
}