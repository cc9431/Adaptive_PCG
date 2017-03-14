using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class HoverCarControl : MonoBehaviour
{
  Rigidbody m_body;
  float m_deadZone = 0.1f;

  public float m_hoverForce = 9.0f;
  public float m_hoverHeight = 2.0f;
  public GameObject[] m_hoverPoints;

  public float m_forwardAcl = 100.0f;
  public float m_backwardAcl = 25.0f;
  float m_currThrust = 0.0f;

  public float m_turnStrength = 10f;
  float m_currTurn = 0.0f;

  public GameObject m_leftAirBrake;
  public GameObject m_rightAirBrake;

  int m_layerMask;

  void Start()
  {
    m_body = GetComponent<Rigidbody>();

    m_layerMask = 1 << LayerMask.NameToLayer("Characters");
    m_layerMask = ~m_layerMask;
  }

  void OnDrawGizmos()
  {

    //  Hover Force
    RaycastHit hit;
    for (int i = 0; i < m_hoverPoints.Length; i++)
    {
      var hoverPoint = m_hoverPoints [i];
      if (Physics.Raycast(hoverPoint.transform.position, 
                          -Vector3.up, out hit,
                          m_hoverHeight, 
                          m_layerMask))
      {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(hoverPoint.transform.position, hit.point);
        Gizmos.DrawSphere(hit.point, 0.5f);
      } else
      {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(hoverPoint.transform.position, 
                       hoverPoint.transform.position - Vector3.up * m_hoverHeight);
      }
    }
  }
	
  void Update()
  {

    // Main Thrust
    m_currThrust = 0.0f;
    float aclAxis = Input.GetAxis("Vertical");
    if (aclAxis > m_deadZone)
      m_currThrust = aclAxis * m_forwardAcl;
    else if (aclAxis < -m_deadZone)
      m_currThrust = aclAxis * m_backwardAcl;

    // Turning
    m_currTurn = 0.0f;
    float turnAxis = Input.GetAxis("Horizontal");
    if (Mathf.Abs(turnAxis) > m_deadZone)
      m_currTurn = turnAxis;
  }

  void FixedUpdate()
  {

    //  Hover Force
    RaycastHit hit;
    for (int i = 0; i < m_hoverPoints.Length; i++)
    {
      var hoverPoint = m_hoverPoints [i];
      if (Physics.Raycast(hoverPoint.transform.position, 
                          -Vector3.up, out hit,
                          m_hoverHeight,
                          m_layerMask))
        m_body.AddForceAtPosition(Vector3.up 
          * m_hoverForce
          * (1.0f - (hit.distance / m_hoverHeight)), 
                                  hoverPoint.transform.position);
      else
      {
        if (transform.position.y > hoverPoint.transform.position.y)
          m_body.AddForceAtPosition(
            hoverPoint.transform.up * m_hoverForce,
            hoverPoint.transform.position);
        else
          m_body.AddForceAtPosition(
            hoverPoint.transform.up * -m_hoverForce,
            hoverPoint.transform.position);
      }
    }

    // Forward
    if (Mathf.Abs(m_currThrust) > 0)
      m_body.AddForce(transform.forward * m_currThrust);

    // Turn
    if (m_currTurn > 0)
    {
      m_body.AddRelativeTorque(Vector3.up * m_currTurn * m_turnStrength);
    } else if (m_currTurn < 0)
    {
      m_body.AddRelativeTorque(Vector3.up * m_currTurn * m_turnStrength);
    }
  }
}
