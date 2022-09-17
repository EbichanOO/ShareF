#import <CoreLocation/CoreLocation.h>
int getiOSLocationPermissionAuthorizationStatus() {
    CLAuthorizationStatus status = [CLLocationManager authorizationStatus];
    switch (status) {
        case kCLAuthorizationStatusNotDetermined:
            return 0;
        break;

        case kCLAuthorizationStatusAuthorizedAlways:
            return 1;
        break;

        case kCLAuthorizationStatusAuthorizedWhenInUse:
            return 2;
        break;

        case kCLAuthorizationStatusRestricted:
            return 3;
        break;

        case kCLAuthorizationStatusDenied:
            return 4;
        break;
    }
}