﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using HoloToolkit.Sharing;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Broadcasts the head transform of the local user to other users in the session,
    /// and adds and updates the head transforms of remote users.  
    /// Head transforms are sent and received in the local coordinate space of the GameObject
    /// this component is on.  
    /// </summary>
    public class RemoteHeadManager : Singleton<RemoteHeadManager>
    {
        public class RemoteHeadInfo
        {
            public long UserID;
            public GameObject HeadObject;
        }

        /// <summary>
        /// Keep a list of the remote heads, indexed by XTools userID
        /// </summary>
        Dictionary<long, RemoteHeadInfo> remoteHeads = new Dictionary<long, RemoteHeadInfo>();

        private void Start()
        {
            CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.HeadTransform] = this.UpdateHeadTransform;

            SharingStage.Instance.SessionUsersTracker.UserJoined += UserJoinedSession;
            SharingStage.Instance.SessionUsersTracker.UserLeft += UserLeftSession;
        }

        private void Update()
        {
            // Grab the current head transform and broadcast it to all the other users in the session
            Transform headTransform = Camera.main.transform;

            // Transform the head position and rotation from world space into local space
            Vector3 headPosition = this.transform.InverseTransformPoint(headTransform.position);
            Quaternion headRotation = Quaternion.Inverse(this.transform.rotation) * headTransform.rotation;

            CustomMessages.Instance.SendHeadTransform(headPosition, headRotation);
        }

        /// <summary>
        /// Called when a new user is leaving the current session.
        /// </summary>
        /// <param name="user">User that left the current session.</param>
        private void UserLeftSession(User user)
        {
            int userId = user.GetID();
            if (userId != SharingStage.Instance.Manager.GetLocalUser().GetID())
            {
                RemoveRemoteHead(this.remoteHeads[userId].HeadObject);
                this.remoteHeads.Remove(userId);
            }
        }

        /// <summary>
        /// Called when a user is joining the current session.
        /// </summary>
        /// <param name="user">User that joined the current session.</param>
        private void UserJoinedSession(User user)
        {
            if (user.GetID() != SharingStage.Instance.Manager.GetLocalUser().GetID())
            {
                GetRemoteHeadInfo(user.GetID());
            }
        }

        /// <summary>
        /// Gets the data structure for the remote users' head position.
        /// </summary>
        /// <param name="userId">User ID for which the remote head info should be obtained.</param>
        /// <returns>RemoteHeadInfo for the specified user.</returns>
        public RemoteHeadInfo GetRemoteHeadInfo(long userId)
        {
            RemoteHeadInfo headInfo;

            // Get the head info if its already in the list, otherwise add it
            if (!this.remoteHeads.TryGetValue(userId, out headInfo))
            {
                headInfo = new RemoteHeadInfo();
                headInfo.UserID = userId;
                headInfo.HeadObject = CreateRemoteHead();

                this.remoteHeads.Add(userId, headInfo);
            }

            return headInfo;
        }

        /// <summary>
        /// Called when a remote user sends a head transform.
        /// </summary>
        /// <param name="msg"></param>
        void UpdateHeadTransform(NetworkInMessage msg)
        {
            // Parse the message
            long userID = msg.ReadInt64();

            Vector3 headPos = CustomMessages.Instance.ReadVector3(msg);

            Quaternion headRot = CustomMessages.Instance.ReadQuaternion(msg);

            RemoteHeadInfo headInfo = GetRemoteHeadInfo(userID);
            headInfo.HeadObject.transform.localPosition = headPos;
            headInfo.HeadObject.transform.localRotation = headRot;
        }

        /// <summary>
        /// Creates a new game object to represent the user's head.
        /// </summary>
        /// <returns></returns>
        GameObject CreateRemoteHead()
        {
            GameObject newHeadObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newHeadObj.transform.parent = this.gameObject.transform;
            newHeadObj.transform.localScale = Vector3.one * 0.2f;
            return newHeadObj;
        }

        /// <summary>
        /// When a user has left the session this will cleanup their
        /// head data.
        /// </summary>
        /// <param name="remoteHeadObject"></param>
        void RemoveRemoteHead(GameObject remoteHeadObject)
        {
            DestroyImmediate(remoteHeadObject);
        }
    }
}