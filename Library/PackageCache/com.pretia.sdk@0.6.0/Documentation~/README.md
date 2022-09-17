# AR Cloud Tracking API

## Overview

### Cloud Anchors
A cloud anchor is a point in space that let you share an AR experience with people around you. A cloud anchor can be a specific marker or an entire room. 

Each user who wishes to participate in a shared experience needs to find the same cloud anchor. Once users have found a cloud anchor, they can share virtual objects with other users. These shared virtual objects will appear at the same physical location.

In AR Cloud Tracking API, a cloud anchor is associated to either a marker, or a map. Each marker contains one cloud anchor, and each map contains one cloud anchor.

### Marker-based cloud anchors
The simplest form of cloud anchor is a marker. A marker is an image (like a poster, or a picture printed on paper) that has been entered in your app database.

Once a user has detected the marker by pointing their device at it, the system automatically uses the associated cloud anchor to enable content sharing.

### Map-based cloud anchor

A map represents an area (for example, a room, a street, etc.) that has been mapped previously. The map can be retrieved from the cloud, or downloaded beforehand and placed in the app database.

Once a map has been loaded into the tracking API, the system can start searching for the cloud anchor.

In the search process, the user only has to point their device around them until the system has recognized the area. At this point, the cloud anchor associated to the map has been found, and the user can start sharing content.

### Virtual Camera
When the tracking API finds a cloud anchor, the virtual camera is automatically attached to it. This creates a one-time "jump" of the camera position. After this point, the camera will stay attached to that anchor, and all virtual objects will also automatically be attached to it.

Note that this jump happens when a cloud anchor is found, which is the case if the system finds a different marker (in case of marker-based cloud anchor), or if a different map is loaded into the system (in case of map-based cloud anchor).

## API Reference

* [Configuration](classPretiaArCloud_1_1ArCloudSessionConfig.html)
* [Cloud Anchor API](classPretiaArCloud_1_1Relocalization.html)

