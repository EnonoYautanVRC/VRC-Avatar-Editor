﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRCAvatarEditor
{
    public static class VRCAvatarConstants
    {
        public const string FX_LEFT_HAND_LAYER_NAME = "Left Hand";
        public static readonly string[] FX_LEFT_HAND_LAYER_NAME_PATTERNS = {
            FX_LEFT_HAND_LAYER_NAME,
            "LeftHand", // Shaclo, Reeva, Leeme
        };
        public const string FX_RIGHT_HAND_LAYER_NAME = "Right Hand";
        public static readonly string[] FX_RIGHT_HAND_LAYER_NAME_PATTERNS = {
            FX_RIGHT_HAND_LAYER_NAME,
            "RightHand", // Shaclo, Reeva, Leeme
        };
        public const string IDLE_STATE_NAME = "Idle";
        public const string EMPTY_ANIMATION_NAME = "Empty";
        public const string RESET_LAYER_NAME = "Reset";
        public const string RESET_FACE_STATE_NAME = "Reset";
        public const string DEFAULT_FACE_ANIMATION_NAME = "DefaultFace";

        public const string OFFICIAL_ANIMATION_PREFIX = "proxy_";
    }
}

