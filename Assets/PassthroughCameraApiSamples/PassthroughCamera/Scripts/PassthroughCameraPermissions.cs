/*// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using System.Linq;
using Meta.XR.Samples;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
using PCD = PassthroughCameraSamples.PassthroughCameraDebugger;

#endif

namespace PassthroughCameraSamples
{
    /// <summary>
    /// PLEASE NOTE: Unity doesn't support requesting multiple permissions at the same time with <see cref="Permission.RequestUserPermissions"/> on Android.
    /// This component is a sample and shouldn't be used simultaneously with other scripts that manage Android permissions.
    /// </summary>
    [MetaCodeSample("PassthroughCameraApiSamples-PassthroughCamera")]
    public class PassthroughCameraPermissions : MonoBehaviour
    {
        [SerializeField] public List<string> PermissionRequestsOnStartup = new() { OVRPermissionsRequester.ScenePermission };

        public static readonly string[] CameraPermissions =
        {
            "android.permission.CAMERA",          // Required to use WebCamTexture object.
            "horizonos.permission.HEADSET_CAMERA" // Required to access the Passthrough Camera API in Horizon OS v74 and above.
        };

        public static bool? HasCameraPermission { get; private set; }
        private static bool s_askedOnce;

#if UNITY_ANDROID
        /// <summary>
        /// Request camera permission if the permission is not authorized by the user.
        /// </summary>
        public void AskCameraPermissions()
        {
            if (s_askedOnce)
            {
                return;
            }
            s_askedOnce = true;
            if (IsAllCameraPermissionsGranted())
            {
                HasCameraPermission = true;
                PCD.DebugMessage(LogType.Log, "PCA: All camera permissions granted.");
            }
            else
            {
                var useCallbacks = false;
                if (!useCallbacks)
                {
                    // We do not have permission to use the microphone.
                    // Check whether you need to display the rationale for requesting permission
                    foreach (var permission in CameraPermissions)
                    {

                        if (Permission.ShouldShowRequestPermissionRationale(permission))
                        {
                            // Show a message or inform the user in other ways why your application needs the microphone permission.
                        }
                        // Ask for permission or proceed without the functionality enabled.
                        Permission.RequestUserPermission(permission);
                    }
                }
                else
                {
                    PCD.DebugMessage(LogType.Log, "PCA: Requesting camera permissions.");

                    var callbacks = new PermissionCallbacks();
                    callbacks.PermissionDenied += PermissionCallbacksPermissionDenied;
                    callbacks.PermissionGranted += PermissionCallbacksPermissionGranted;

                    // It's important to request all necessary permissions in one request because only one 'PermissionCallbacks' instance is supported at a time.
                    var allPermissions = CameraPermissions.Concat(PermissionRequestsOnStartup).ToArray();
                    Permission.RequestUserPermissions(allPermissions, callbacks);
                }
            }
        }

        /// <summary>
        /// Permission Granted callback
        /// </summary>
        /// <param name="permissionName"></param>
        private static void PermissionCallbacksPermissionGranted(string permissionName)
        {
            PCD.DebugMessage(LogType.Log, $"PCA: Permission {permissionName} Granted");

            // Only initialize the WebCamTexture object if both permissions are granted
            if (IsAllCameraPermissionsGranted())
            {
                HasCameraPermission = true;
            }
        }

        /// <summary>
        /// Permission Denied callback.
        /// </summary>
        /// <param name="permissionName"></param>
        private static void PermissionCallbacksPermissionDenied(string permissionName)
        {
            PCD.DebugMessage(LogType.Warning, $"PCA: Permission {permissionName} Denied");
            HasCameraPermission = false;
            s_askedOnce = false;
        }

        private static bool IsAllCameraPermissionsGranted() => CameraPermissions.All(Permission.HasUserAuthorizedPermission);
#endif
    }
}*/



/* Meta Approved Version but throws a warning. Otherwise functional. I really wanna keep this as fallback.
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 */


using System.Collections.Generic;
using System.Linq;
using Meta.XR.Samples;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
using PCD = PassthroughCameraSamples.PassthroughCameraDebugger;

#endif

namespace PassthroughCameraSamples
{
    /// <summary>
    /// PLEASE NOTE: Unity doesn't support requesting multiple permissions at the same time with <see cref="Permission.RequestUserPermissions"/> on Android.
    /// This component is a sample and shouldn't be used simultaneously with other scripts that manage Android permissions.
    /// </summary>
    [MetaCodeSample("PassthroughCameraApiSamples-PassthroughCamera")]
    public class PassthroughCameraPermissions : MonoBehaviour
    {
        [SerializeField] public List<string> PermissionRequestsOnStartup = new() { OVRPermissionsRequester.ScenePermission };

        public static readonly string[] CameraPermissions =
        {
            "android.permission.CAMERA",          // Required to use WebCamTexture object.
            "horizonos.permission.HEADSET_CAMERA" // Required to access the Passthrough Camera API in Horizon OS v74 and above.
        };

        public static bool? HasCameraPermission { get; private set; }
        private static bool s_askedOnce;
        
#if UNITY_ANDROID
        /// <summary>
        /// Request camera permission if the permission is not authorized by the user.
        /// </summary>
        public void AskCameraPermissions()
        {
            if (s_askedOnce)
            {
                return;
            }
            s_askedOnce = true;
            if (IsAllCameraPermissionsGranted())
            {
                HasCameraPermission = true;
                PCD.DebugMessage(LogType.Log, "PCA: All camera permissions granted.");
            }
            else
            {
                PCD.DebugMessage(LogType.Log, "PCA: Requesting camera permissions.");

                var callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += PermissionCallbacksPermissionDenied;
                callbacks.PermissionGranted += PermissionCallbacksPermissionGranted;
                callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacksPermissionDenied;

                // It's important to request all necessary permissions in one request because only one 'PermissionCallbacks' instance is supported at a time.
                var allPermissions = CameraPermissions.Concat(PermissionRequestsOnStartup).ToArray();
                Permission.RequestUserPermissions(allPermissions, callbacks);
            }
        }

        /// <summary>
        /// Permission Granted callback
        /// </summary>
        /// <param name="permissionName"></param>
        private static void PermissionCallbacksPermissionGranted(string permissionName)
        {
            PCD.DebugMessage(LogType.Log, $"PCA: Permission {permissionName} Granted");

            // Only initialize the WebCamTexture object if both permissions are granted
            if (IsAllCameraPermissionsGranted())
            {
                HasCameraPermission = true;
            }
        }

        /// <summary>
        /// Permission Denied callback.
        /// </summary>
        /// <param name="permissionName"></param>
        private static void PermissionCallbacksPermissionDenied(string permissionName)
        {
            PCD.DebugMessage(LogType.Warning, $"PCA: Permission {permissionName} Denied");
            HasCameraPermission = false;
            s_askedOnce = false;
        }

        private static bool IsAllCameraPermissionsGranted() => CameraPermissions.All(Permission.HasUserAuthorizedPermission);
#endif
    }
}
