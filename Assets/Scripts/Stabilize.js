/*var WheelL : WheelCollider;
var WheelR : WheelCollider;
var AntiRoll = 5000.0;
 
function FixedUpdate ()
    {
    var hit : WheelHit;
    var travelL = 1.0;
    var travelR = 1.0;
 
    var groundedL = WheelL.GetGroundHit(hit);
    if (groundedL)
        travelL = (-WheelL.transform.InverseTransformPoint(hit.point).y - WheelL.radius) / WheelL.suspensionDistance;
 
    var groundedR = WheelR.GetGroundHit(hit);
    if (groundedR)
        travelR = (-WheelR.transform.InverseTransformPoint(hit.point).y - WheelR.radius) / WheelR.suspensionDistance;
 
    var antiRollForce = (travelL - travelR) * AntiRoll;
 
    if (groundedL)
        rigidbody.AddForceAtPosition(WheelL.transform.up * -antiRollForce,
               WheelL.transform.position); 
    if (groundedR)
        rigidbody.AddForceAtPosition(WheelR.transform.up * antiRollForce,
               WheelR.transform.position); 
    }*/