using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// A list of criteria to select a map from the server
/// </summary>
public class MapSelectionCriteria
{
    public enum PublicStatus
    {
        /// <summary> Map can be either private or public </summary>
        ANY,
        /// <summary> Map can only be public </summary>
        PUBLIC_ONLY,
        /// <summary> Map can only be private </summary>
        PRIVATE_ONLY
    }

    public enum MergeStatus
    {
        /// <summary> Map can be either merged or not merged </summary>
        ANY,
        /// <summary> Map can only be merged </summary>
        MERGED_MAPS_ONLY,
        /// <summary> Map can only be not merged </summary>
        NOT_MERGED_MAPS_ONLY,
        /// <summary> Only merged maps are selected if at least one exists meeting the other criteria, otherwise not merged maps are selected </summary>
        MERGED_MAPS_IF_EXISTS
    }

    public const float TIME_THRESHOLD_MAX = 1.0f;
    public const float TIME_THRESHOLD_STANDARD = 0.16f;

    public enum ResultOrder
    {
        /// <summary> Selected maps are not sorted </summary>
        NO_SORTING,
        /// <summary> Selected maps are sorted from closest to furthest based on GPS distance </summary>
        SORT_BY_GPS_DISTANCE,
        /// <summary> Selected maps are sorted from closest to furthest based on time of day </summary>
        SORT_BY_TIME
    }

    [Tooltip("Select maps based on public/private status")]
    /// <summary>
    /// Select maps based on public/private status
    /// </summary>
    public PublicStatus Public = PublicStatus.ANY;

    [Tooltip("Select maps based on merge status")]
    /// <summary>
    /// Select maps based on merge status
    /// </summary>
    public MergeStatus Merged = MergeStatus.ANY;

    [Tooltip("Select maps that belongs to one of these groups. If the list is empty, select from any group.")]
    /// <summary>
    /// Select maps that belongs to one of these groups. If the list is empty, select from any group.
    /// </summary>
    public List<string> GroupKeys = new List<string>();

    [Tooltip("Select maps that are closer than this distance (in meters), based on current GPS location")]
    /// <summary>
    /// Select maps that are closer than this distance (in meters), based on current GPS location
    /// </summary>
    public float GpsThreshold = float.MaxValue;

    [Range(0f, 1f)]
    [Tooltip("Select maps that are closer in time than this threshold, based on current time. This time distance is normalized in [0..1]. Closer to 0 = maps is closer to current daylight conditions.")]
    /// <summary>
    /// Select maps that are closer in time than this threshold, based on current time.
    /// This time distance is normalized in [0..1]. Closer to 0 = maps is closer to current daylight conditions.
    /// </summary>
    public float TimeThreshold = TIME_THRESHOLD_MAX;

    [Tooltip("Sort the selected maps")]
    /// <summary>
    /// Sort the selected maps
    /// </summary>
    public ResultOrder Sorting = ResultOrder.SORT_BY_GPS_DISTANCE;


    
    /// <summary>
    /// Create a criteria object that selects maps in the local area, and sort the results by time
    /// </summary>
    /// <param name="distance">Area radius (in meters) around current location</param>
    public static MapSelectionCriteria MapsInArea(float distance = 50.0f, List<string> groupKeys = null)
    {
        return new MapSelectionCriteria
        {
            GroupKeys = (groupKeys == null) ? new List<string>() : groupKeys,
            GpsThreshold = distance,
            TimeThreshold = TIME_THRESHOLD_MAX,
            Sorting = ResultOrder.SORT_BY_TIME
        };
    }

    /// <summary>
    /// Create a criteria object that selects all available maps and sort the results by GPS distance
    /// </summary>
    public static MapSelectionCriteria AllMaps(List<string> groupKeys = null)
    {
        return new MapSelectionCriteria
        {
            GroupKeys = (groupKeys == null) ? new List<string>() : groupKeys,
            GpsThreshold = float.MaxValue,
            TimeThreshold = TIME_THRESHOLD_MAX,
            Sorting = ResultOrder.SORT_BY_GPS_DISTANCE
        };
    }
}
