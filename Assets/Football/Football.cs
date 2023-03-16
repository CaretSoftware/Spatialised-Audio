using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Football : MonoBehaviour {
    private const int NumPlayersOnATeam = 7;
    
    [SerializeField] private Transform[] defenders;
    [SerializeField] private Transform[] attackers;
    
    [SerializeField] private float xPositionCenterLine = 0;
    [SerializeField] private Vector3 ballPosition;
    
    [SerializeField] private int[] attackersDefenderIndex = new int[NumPlayersOnATeam];
    
    private Heap<PlayerDistance> _sortedPlayerDistances;

    // sorts attackers by how far forward they are (greater position.z = greater priority)
    private void SortAttackersByForwardness() {
        SortAttackersBy(Comparer<Transform>.Create((a, b) => -a.position.z.CompareTo(b.position.z)));
    }

    // sorts attackers by how far they are from ball position (ballPosition is defined as a class variable)
    private void SortAttackersByDistanceToBall() {
        SortAttackersBy(Comparer<Transform>.Create((a, b) => -(a.position - ballPosition).sqrMagnitude.CompareTo((b.position - ballPosition).sqrMagnitude)));
    }
    
    // sorts attackers by how far they are from the centerline (centerline is defined as a class variable)
    private void SortAttackersByDistanceFromCenterline() {
        SortAttackersBy(Comparer<Transform>.Create((a, b) =>
            (Mathf.Abs(a.position.x - xPositionCenterLine)).CompareTo(Mathf.Abs(b.position.x - xPositionCenterLine))));
    }

    // sorts attackers by the comparator passed in
    private void SortAttackersBy(Comparer<Transform> comparer) {
        Heap<Transform> attackersSortedByForwardness = 
            new Heap<Transform>(
                comparer,
                //Comparer<Transform>.Create((a, b) => (Mathf.Abs(a.position.x - xPositionCenterLine)).CompareTo(Mathf.Abs(b.position.x - xPositionCenterLine))), 
                //Comparer<Transform>.Create((a, b) => a.position.z.CompareTo(b.position.z)), // sorts by forwardness
                attackers.Length);

        // push all attackers into heap
        for (int attacker = 0; attacker < attackers.Length; attacker++) {
            attackersSortedByForwardness.Insert(attackers[attacker]);
        }

        // pop all attackers - they come out in sorted order
        // insert them back into the attacker array
        for (int attacker = 0; attacker < attackers.Length; attacker++) {
            attackers[attacker] = attackersSortedByForwardness.DeleteMin();
        }
    }
    
    private void PutAllDistancesInAHeap() {

        _sortedPlayerDistances = new Heap<PlayerDistance>();
        for (int attacker = 0; attacker < attackers.Length; attacker++) {
            for (int defender = 0; defender < defenders.Length; defender++) {
                PlayerDistance playerDistance = new PlayerDistance(defender, attacker, defenders, attackers);
                _sortedPlayerDistances.Insert(playerDistance);
            }
        }
    }

    private void AssignDefendersToAttackers() {
        bool[] attackerHasAssignedDefender = new bool[NumPlayersOnATeam];
        bool[] defenderHasAssignedAttacker = new bool[NumPlayersOnATeam];
        int safetyInt = 0;
        
        int index = 0;
        while (index < NumPlayersOnATeam && safetyInt++ < 10000) {
            if (_sortedPlayerDistances == null || _sortedPlayerDistances.Empty()) break;

            PlayerDistance closestDistance = _sortedPlayerDistances.DeleteMin();

            int attackerIndex = closestDistance.attackerIndex;
            int defenderIndex = closestDistance.defenderIndex;

            if (attackerHasAssignedDefender[attackerIndex] || defenderHasAssignedAttacker[defenderIndex])
                continue;

            attackerHasAssignedDefender[attackerIndex] = true;
            defenderHasAssignedAttacker[defenderIndex] = true;
            index++;
            
            attackersDefenderIndex[attackerIndex] = defenderIndex;
        }
    }
    
#if UNITY_EDITOR // cannot compile during builds with OnDrawGizmos method calls, this has the compiler ignore it during builds
    private void OnDrawGizmos() {
        //SortAttackersByForwardness(); // if you want to prioritise targets for the most forward attackers
        //SortAttackersByDistanceFromCenterline(); // if you want to prioritise targets for the attackers closest to the center
        SortAttackersByDistanceToBall(); // if you want to prioritise targets for the attackers closest to the ball
        PutAllDistancesInAHeap();
        AssignDefendersToAttackers();

        // draw ball position
        Gizmos.DrawWireSphere(ballPosition, .3f);
        Handles.Label(ballPosition, nameof(ballPosition));
        
        if (attackersDefenderIndex != null) {
            for (int attackerIndex = 0; attackerIndex < attackersDefenderIndex.Length; attackerIndex++) {
                
                // here the index is used to draw a line between the players - you can use the index to assign a waypoint target from an array/list or whatever you need
                // line from attacker to defender
                int defenderIndex = attackersDefenderIndex[attackerIndex];
                Transform defender = defenders[defenderIndex];
                Transform attacker = attackers[attackerIndex];
                Vector3 to = defender.position;
                Vector3 from = attacker.position;
                Debug.DrawLine(from, to, Color.red);

                // arrow pointers
                Vector3 lineDir = (to - from).normalized;
                Quaternion rotation = Quaternion.Euler(lineDir);
                Quaternion fortyFive = rotation * Quaternion.Euler(0f, 45f, 0f);
                Quaternion negFortyFive = rotation * Quaternion.Euler(0f, -45f, 0f);
                Debug.DrawLine(to, to - (fortyFive * lineDir), Color.red);
                Debug.DrawLine(to, to - (negFortyFive * lineDir), Color.red);
            }
        }
    }
#endif

    [System.Serializable]   // System.Serializable so we can save them to an array or list outside of runtime
    // class PlayerDistance to be compatible with the heap for sorting
    private class PlayerDistance : IComparable {

        public int defenderIndex;
        public int attackerIndex;
        public float distance;

        public PlayerDistance(int defenderIndex, int attackerIndex, Transform[] defenders, Transform[] attackers) {
            Transform defender = defenders[defenderIndex];
            Transform attacker = attackers[attackerIndex];
            Vector3 defPos = defender.position;
            Vector3 attPos = attacker.position;
            
            this.defenderIndex = defenderIndex;
            this.attackerIndex = attackerIndex;
            distance = (defPos - attPos).sqrMagnitude;
        }
        
        public int CompareTo(PlayerDistance other) {
            if (other == this || this.distance == other.distance) return 0; // loss of precision during comparison of floating point numbers, but, meh... ¯\_(ツ)_/¯

            if (this.distance > other.distance)
                return 1;
            else
                return -1;
        }

        public int CompareTo(object obj) {
            if (!(obj is PlayerDistance)) {
                throw new TypeAccessException("Can only be compared with other PlayerDistance classes");
            }

            return CompareTo((PlayerDistance)obj);
        }
    }
}
