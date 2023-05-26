using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.Serialization;

namespace GravityVisualiser
{
    public class GravitationalObject : MonoBehaviour
    {
        //Contains every GravitationalObject that is an attractor
        //(Should be accounted in gravity calculations)
        public static List<GravitationalObject> Attractors = new List<GravitationalObject>();
        public const float gravitationalConstant = 6.674e-11f;


        [SerializeField] private float mass;

        [Header("Object to orbit on start of play")] [SerializeField]
        private GravitationalObject orbitAtStart;

        [Header("Should be accounted in gravity calculations")] [SerializeField]
        private bool isAttractor = false;

        //If -1, it means that this is not an attractor
        private int attractorIndex = -1;
        private bool orbitVelocityCalculated;

        private Vector3 _velocity;

        public Vector3 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        public float Mass
        {
            get => mass;
        }

        public void OnDisable()
        {
            RemoveFromAttractorList();
        }

        public void OnDestroy()
        {
            RemoveFromAttractorList();
        }

        public void OnEnable()
        {
            if (isAttractor)
            {
                Attractors.Add(this);
                RecalculateAttractorIndexes();
            }

            orbitVelocityCalculated = false;
            SetOrbitalVelocity();
        }

        private void RemoveFromAttractorList()
        {
            if (attractorIndex != -1)
            {
                Attractors[attractorIndex] = null;
                attractorIndex = -1;
                RecalculateAttractorIndexes();
            }
        }

        private void RecalculateAttractorIndexes()
        {
            for (int i = 0; i < Attractors.Count; i++)
            {
                if (Attractors[i] == null)
                    Attractors.RemoveAt(i);
            }
            Attractors.Sort((a, b) => b.Mass.CompareTo(a.Mass));

            for (int i = 0; i < Attractors.Count; i++)
            {
                Attractors[i].attractorIndex = i;
            }
        }


        /// <summary>
        /// Calculates and sets the orbital velocity of this object, relative to the object that's set as orbitAtStart
        /// </summary>
        void SetOrbitalVelocity()
        {
            if (!orbitVelocityCalculated && orbitAtStart)
            {
                orbitVelocityCalculated = true;

                Vector3 delta = transform.localPosition - orbitAtStart.transform.localPosition;
                float distance = delta.magnitude;

                delta.y = 0;


                Vector3 direction = delta.normalized;
                Vector3 travelDirection = direction;

                float tempX = travelDirection.x;
                travelDirection.x = -travelDirection.z;
                travelDirection.z = tempX;

                _velocity = Mathf.Sqrt((gravitationalConstant * orbitAtStart.mass / distance)) *
                            travelDirection.normalized;

                if (orbitAtStart.orbitAtStart != null)
                {
                    orbitAtStart.SetOrbitalVelocity();
                }

                _velocity += orbitAtStart._velocity;
            }
        }

        void FixedUpdate()
        {
            Vector3 acl = Vector3.zero;
            Vector3 pos = transform.localPosition;

            
            foreach (GravitationalObject gravObject in Attractors)
            {
                if (gravObject != this)
                {
                    Vector3 delta = gravObject.transform.localPosition - pos;
                    delta.y = 0;
                    float distance = delta.magnitude;
                    
                    Vector3 addedAcl = delta.normalized * gravObject.Attracting(pos, this.mass);

                    acl += addedAcl / this.mass;
                }
            }

            _velocity += acl * (Time.deltaTime * SimulationTimeScaleManager.timeScale);
            pos += _velocity * (Time.deltaTime * SimulationTimeScaleManager.timeScale);
            
            pos.y = 0f;
            transform.localPosition = pos;
        }

        private void LateUpdate()
        {
            Vector3 localPos = transform.localPosition;
            localPos.y = SpaceTimeMath.GetYPosAtPoint(new float2(transform.localPosition.x, transform.localPosition.z), SpaceTimeMath._centPotentialVisiblity, attractorIndex);
            localPos.y += transform.localScale.y / 2f;
            transform.localPosition = localPos;
        }

        public float Attracting(Vector3 pos, float mass = 1)
        {
            Vector3 thisPos = transform.localPosition;
            pos.y = 0;
            thisPos.y = 0;
            float distance = Vector3.Distance(pos, thisPos);
            return gravitationalConstant * ((this.mass * mass) / (distance * distance));
        }
    }
}