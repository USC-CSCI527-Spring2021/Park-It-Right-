using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public static float PARKING_REWARD = 5f;
    public static float OBSTACLE_HIT_REWARD = -0.5f;
    public static float WALL_HIT_REWARD = -0.5f;
    public static float CLOSE_TO_WALL_REWARD = -0.005f;
    public static float CLOSE_TO_OBSTACLE_REWARD = -0.005f;
    public static float CLOSEST_TO_TARGET_REWARD = 0.00003f;
    public static float BEST_DISTANCE_TO_TARGET_REWARD = 0.00002f;
    public static float MOVING_TO_TARGET_REWARD = 0.00001f;
    public static float MOVING_AWAY_TARGET_REWARD = -0.00002f;
    public static float NUMBER_OF_STOREYS_FOR_LEVEL_2 = 2;
    public static float MOVE_TO_CORRECT_STOREY = 0.1f;
    public static float MOVE_TO_INCORRECT_STOREY = -0.1f;
    public static float NUMBER_OF_WALLS_FOR_LEVEL_2 = 4 * NUMBER_OF_STOREYS_FOR_LEVEL_2;
    public static float NUMBER_OF_OBSTACLES_FOR_LEVEL_2 = 2 * NUMBER_OF_STOREYS_FOR_LEVEL_2;
}
