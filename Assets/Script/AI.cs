using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AI : MonoBehaviour 
{
	public Manager m;
	
	static float radius = 0.5f;
	
	Vector3[] hole = {
		new Vector3(-8.35f, 11.25f, -8.35f),
		new Vector3(8.3f, 11.25f, -8.3f),
		new Vector3(8.35f, 11.25f, 8.35f),
		new Vector3(-8.3f, 11.25f, 8.3f)
	};
	
	Vector3[] circle = {
		new Vector3(-6.3f, 11.25f, -6.3f),
		new Vector3(6.3f, 11.25f, -6.3f),
		new Vector3(6.3f, 11.25f, 6.3f),
		new Vector3(-6.3f, 11.25f, 6.3f)
	};
	
	Vector3[] corner = {
		new Vector3(-10f, 11.25f, -10f),
		new Vector3(10f, 11.25f, -10f),
		new Vector3(10f, 11.25f, 6.3f),
		new Vector3(-10f, 11.25f, 10f)
	};
	
	static float circle_r = (7.44f - 5.14f)/2 + radius;
	
	public class ShootInfo {
		public Vector3 position;
		public float angle;
		public float force;
		public bool able;
		public ShootInfo(Vector3 _position, float _angle, float _force, bool _able){
			position = _position;
			angle = _angle;
			force = _force;
			able = _able;
		}
	}
	
	IEnumerator AIShoot(Vector3 shooterPosition, float angle, float force){
		yield return new WaitForSeconds(1);
		m.SetStatus(1);
		m.shooter.transform.position = shooterPosition;
		
		m.shooter.gameObject.SetActive(true);
		
		yield return new WaitForSeconds(1);
		m.cue.gameObject.SetActive(true);
		m.cue.gameObject.transform.position = shooterPosition;
		Vector3 cue_angle = m.cue.gameObject.transform.localEulerAngles;
		if(angle < 0)
			angle = 180 + angle;
		cue_angle.y = (270 - angle);
		m.cue.gameObject.transform.localEulerAngles = cue_angle;
		Quaternion rotation = Quaternion.Euler(new Vector3(0, m.cue.gameObject.transform.localEulerAngles.y, 0));
		Vector3 direction = rotation * Vector3.back;
		Rigidbody rb = m.shooter.GetComponent<Rigidbody>();
		Debug.Log("FORCE = "+force);
		rb.AddForce(direction * force * (1 + Random.Range(-0.05f, 0.05f)));
		m.SetStatus(4);
	}
	
	int ModeDecision(){
		int easyCount = 0;
		int bombCount = 0;
		foreach(Transform child in m.redChess.transform){
			Chess chess = child.gameObject.GetComponent<Chess>();
			if(chess.region == 5)
				bombCount++;
			if(chess.region >= 1 && chess.region <= 4)
				easyCount++;
		}

		if(easyCount > 0)
			return 1;
		else if(bombCount > 0)
			return 0;
		else
			return 2;
	}
	
	public void Hit(){
		float min = 999999;
		Vector3 shooterPosition = new Vector3(0f, 11.25f, 0f);
		
		int mode = ModeDecision();
		float angle = 0;
		float force = 0;
		string targetName = "";
		
		if(mode == 0){
			foreach(Transform child in m.redChess.transform){
				Chess chess = child.gameObject.GetComponent<Chess>();
				if(Distance(child.position, new Vector3(0f, 11.25f, 9.457f)) <= min && chess.region == 5){
					ShootInfo info = BombChess(child);
					if(info.able){
						min = Distance(child.position, new Vector3(0f, 11.25f, 9.457f));
						shooterPosition = info.position;
						angle = info.angle;
						force = info.force;
						targetName = child.gameObject.name;
					}
				}
			}
		}
		else if(mode == 1){
			foreach(Transform child in m.redChess.transform){
				Chess chess = child.gameObject.GetComponent<Chess>();
				if(chess.region >= 1 && chess.region <= 2 && chess.type != 1){
					ShootInfo info = EasyChess(child);
					if(info.able){
						shooterPosition = info.position;
						angle = info.angle;
						force = info.force;
						targetName = child.gameObject.name;
					}
				}
			}
			foreach(Transform child in m.redChess.transform){
				Chess chess = child.gameObject.GetComponent<Chess>();
				if(chess.region >= 3 && chess.region <= 4 && chess.type != 1){
					ShootInfo info = EasyChess(child);
					if(info.able){
						shooterPosition = info.position;
						angle = info.angle;
						force = info.force;
						targetName = child.gameObject.name;
					}
				}
			}
			foreach(Transform child in m.redChess.transform){
				Chess chess = child.gameObject.GetComponent<Chess>();
				if(chess.type == 1){
					ShootInfo info = SmallReturnChess(child);
					if(info.able){
						shooterPosition = info.position;
						angle = info.angle;
						force = info.force;
						targetName = child.gameObject.name;
					}
				}
			}
		}
		else if(mode == 2){
			foreach(Transform child in m.redChess.transform){
				Chess chess = child.gameObject.GetComponent<Chess>();
				if(chess.region == 8){
					ShootInfo info = ReturnChess(child);
					if(info.able){
						shooterPosition = info.position;
						angle = info.angle;
						force = info.force;
						targetName = child.gameObject.name;
					}
				}
			}
			foreach(Transform child in m.redChess.transform){
				Chess chess = child.gameObject.GetComponent<Chess>();
				if(chess.region == 6 || chess.region == 7){
					ShootInfo info = CornerChess(child);
					if(info.able){
						shooterPosition = info.position;
						angle = info.angle;
						force = info.force;
						targetName = child.gameObject.name;
					}
				}
			}
		}
		if(force == 0){
			ShootInfo info = RandomChess();
			shooterPosition = info.position;
			angle = info.angle;
			force = info.force;
		}
		Debug.Log(targetName);
		StartCoroutine(AIShoot(shooterPosition, angle, force));
     
	}
	
	ShootInfo RandomChess(){
		int randomCircle = Random.Range(0, 1);
		float randomX = Random.Range(circle[randomCircle].x - circle_r, circle[randomCircle].x + circle_r);
		float randomY = Random.Range(circle[randomCircle].z - circle_r, circle[randomCircle].z + circle_r);
		float randomXin = 0;
		if(randomCircle == 0)
			randomXin = Random.Range(corner[0].x, circle[0].x);
		else
			randomXin = Random.Range(corner[1].x, circle[1].x);
		float slope = (corner[randomCircle].z - randomY) / (randomXin - randomX);
		float randomAngle = Mathf.Rad2Deg * Mathf.Atan(slope);
		int randomForce = Random.Range(20000000, 60000000);
		return new ShootInfo(new Vector3(randomX, 11.25f, randomY), randomAngle, randomForce, true);
	}
	
	ShootInfo CornerChess(Transform chess){
		float dir = Mathf.Sign(chess.position.x);
		int fromCircle, toHole;
			
		if(dir > 0){
			fromCircle = 1;
			toHole = 2;
		}
		else{
			fromCircle = 0;
			toHole = 3;
		}
		Vector3 target = ShiftDistance(hole[toHole], chess.position, -radius*2f);
		//target.x -= 0.5f;

		
		float slope2 = Slope(circle[fromCircle], target);
		float angle2 = Mathf.Rad2Deg * Mathf.Atan(slope2);
		Vector3 shooterPosition = LineSegCircleIntersection(target, circle[fromCircle], circle[fromCircle], circle_r);
		if(TestRoute(target, circle[fromCircle], chess.gameObject.name)){
			return new ShootInfo(shooterPosition, angle2, 45000000, true);
		}
		else{
			return new ShootInfo(new Vector3(0, 0, 0), 0, 0, false);
		}
	}
	
	
	ShootInfo BombChess(Transform chess){
		float dir = Mathf.Sign(chess.position.x);
		int fromCircle;
			
		if(dir > 0)
			fromCircle = 0;
		else
			fromCircle = 1;
		
		int chessCount = 1;
		foreach(Transform child in m.redChess.transform){
			Chess script = child.gameObject.GetComponent<Chess>();
			if(script.region == 5 && child.gameObject.name != chess.gameObject.name){
				if(dir > 0){
					if(child.position.x > chess.position.x)
						chessCount++;
				}
				else{
					if(child.position.x < chess.position.x){
						chessCount++;
					}
				}
			}
		}
			
		Vector3 target = ShiftDistance(chess.position, dir * 2f, dir * radius*2);

		for(float t = 0; t <= 2f; t += 0.2f){
			Vector3 circlePoint = circle[fromCircle];
			circlePoint.x = circlePoint.x - circle_r + circle_r * t;
			float slope2 = Slope(circlePoint, target);
			float angle2 = Mathf.Rad2Deg * Mathf.Atan(slope2);
			float xin = (corner[fromCircle].z - circlePoint.z) / slope2 + circlePoint.x;
			if(Mathf.Abs(xin) <= Mathf.Abs(corner[fromCircle].z)){
				if(TestRoute(target, circlePoint, chess.gameObject.name)){
					float force = chessCount * 7000000 + 30000000;
					return new ShootInfo(circlePoint, angle2, force, true);
				}
			}
		}
		return new ShootInfo(new Vector3(0, 0, 0), 0, 0, false);
		//}
	}
	
	ShootInfo EasyChess(Transform chess){
		Chess script = chess.gameObject.GetComponent<Chess>();

		int fromCircle = 0, toHole = 0;
		if(script.region == 1){
			fromCircle = 0;
			toHole = 2;
		}
		else if(script.region == 2){
			fromCircle = 1;
			toHole = 3;
		}
		else if(script.region == 3){
			fromCircle = 0;
			toHole = 3;
		}
		else if(script.region == 4){
			fromCircle = 1;
			toHole = 2;
		}
			
		
		float slope1 = Slope(hole[toHole], chess.position);

		if(TestRoute(chess.position, hole[toHole], chess.gameObject.name)){
					
			Vector3 target = ShiftDistance(hole[toHole], chess.position, radius*2);

			float slope2 = Slope(circle[fromCircle], target);
			float angle = Mathf.Rad2Deg * Mathf.Atan(Mathf.Abs((slope1 - slope2) / (1f + slope1 * slope2)));
			float angle2 = Mathf.Rad2Deg * Mathf.Atan(slope2);
			
			float minAngle = 999;
			Vector3 bestCirclePoint = new Vector3(0f, 0f, 0f);
			float bestAngle = 999;
			
			for(float t = 0; t <= 2f; t += 0.2f){
				Vector3 circlePoint = circle[fromCircle];
				circlePoint.x = circlePoint.x - circle_r + circle_r * t;
				slope2 = Slope(circlePoint, target);
				float xin = (corner[fromCircle].z - circlePoint.z) / slope2 + circlePoint.x;
				if(Mathf.Abs(xin) <= Mathf.Abs(corner[fromCircle].z)){
					angle = Mathf.Rad2Deg * Mathf.Atan(Mathf.Abs((slope1 - slope2) / (1f + slope1 * slope2))); 
					angle2 = Mathf.Rad2Deg * Mathf.Atan(slope2);
					if(TestRoute(target, circlePoint, chess.gameObject.name)){
						if(angle < minAngle){
							minAngle = angle;
							bestCirclePoint = circlePoint;
							bestAngle = angle2;
						}
					}
				}
			}
			
			if(minAngle == 999)
				return new ShootInfo(new Vector3(0, 0, 0), 0, 0, false);
			else{
				float force = (Distance(bestCirclePoint, target) + 1.5f * Distance(hole[toHole], chess.position)) * (1f + Mathf.Sin(Mathf.Deg2Rad*minAngle));
				return new ShootInfo(bestCirclePoint, bestAngle, 15000000 + force * 900000, true);
			}
		}
		else
			return new ShootInfo(new Vector3(0, 0, 0), 0, 0, false);
	}
	
	ShootInfo ReturnChess(Transform chess){
		int fromCircle;
		float dir = Mathf.Sign(chess.position.x);
		if(dir > 0)
			fromCircle = 0;
		else
			fromCircle = 1;
		
		Vector3 target = new Vector3(0f, 11.25f, 10f);
		Vector3 chessPosition = chess.position;
		chessPosition.x = chess.position.x - dir * radius;
			for(float t = 0; t <= 2f; t += 0.5f){
				Vector3 circlePoint = circle[fromCircle];
				circlePoint.x = circlePoint.x - circle_r + circle_r * t;
				float dy1 = 10 - circlePoint.z;
				float dy2 = 10 - chessPosition.z;
				target.x = (circlePoint.x * dy2 + chessPosition.x * dy1) / (dy1 + dy2);

				float slope = Slope(circlePoint, target);
				float angle = Mathf.Rad2Deg * Mathf.Atan(slope);
				float xin = (corner[fromCircle].z - circlePoint.z) / slope + circlePoint.x;
				if(xin >= corner[fromCircle].z){
					if(TestRoute(circlePoint, target, chess.gameObject.name)){
						if(TestRoute(target, chessPosition, chess.gameObject.name)){
							float force = Distance(circlePoint, target) + Distance(target, chessPosition);
							return new ShootInfo(circlePoint, angle, 90000000, true);
						}
					}
				}
			}
		
		return new ShootInfo(new Vector3(0, 0, 0), 0, 0, false);
	}
	
	ShootInfo SmallReturnChess(Transform chess){
		for(int fromCircle = 0; fromCircle <= 1; fromCircle++){
				Vector3 circlePoint = circle[fromCircle];
				Vector3 target = new Vector3(0, 0, 0);
				circlePoint.z -= radius;
				float x = 0;
				if(fromCircle == 0){
					x = -10;
					target = new Vector3(-10f, 11.25f, 0f);
				}
				else{
					x = 10;
					target = new Vector3(10f, 11.25f, 0f);
				}
				float dx1 = x - circlePoint.x;
				float dx2 = x - chess.position.x;
				target.z = (circlePoint.z * dx2 + chess.position.z * dx1) / (dx1 + dx2);

				float slope = Slope(circlePoint, target);
				float angle = Mathf.Rad2Deg * Mathf.Atan(slope);
				float xin = (corner[fromCircle].z - circlePoint.z) / slope + circlePoint.x;
				if(xin >= corner[fromCircle].z){
					if(TestRoute(circlePoint, target, chess.gameObject.name)){
						if(TestRoute(target, chess.position, chess.gameObject.name)){
							float force = Distance(circlePoint, target) + Distance(target, chess.position);
							return new ShootInfo(circlePoint, angle, 70000000, true);
						}
					}
				}
			
		}
		return new ShootInfo(new Vector3(0, 0, 0), 0, 0, false);
	}
	
	float Slope(Vector3 a, Vector3 b){
		return (a.z - b.z) / (a.x - b.x);
	}
	float Distance(Vector3 a, Vector3 b){
		return Mathf.Pow(Mathf.Pow(a.x - b.x, 2f) + Mathf.Pow(a.z - b.z, 2f), 0.5f);
	}
	Vector3 LineCircleIntersection(float slope, float yin, Vector3 center, float r){
		float a = slope * slope + 1;
		float b = 2 * (slope * yin - slope * center.z - center.x);
		float c = center.x * center.x + center.z * center.z - r * r - 2 * yin * center.z + yin * yin;
		float discriminant = b * b - 4 * a * c;
		Vector3 shooterPosition = new Vector3(0f, 0f, 0f);

		if(discriminant >= 0){
			shooterPosition.x = (-b + Mathf.Pow(discriminant, 0.5f)) / (2*a);
			shooterPosition.z = slope * shooterPosition.x + yin;
			shooterPosition.y = 11.25f;
		}
		else{
			shooterPosition.y = 0;
		}
		return shooterPosition;
	}
	
	Vector3 LineSegCircleIntersection(Vector3 p1, Vector3 p2, Vector3 center, float r){
		float a = Mathf.Pow(p1.x - p2.x, 2f) + Mathf.Pow(p1.z - p2.z, 2f);
		float b = 2 * ((p1.x - p2.x) * (p2.x - center.x) + (p1.z - p2.z) * (p2.z - center.z));
		float c = Mathf.Pow(p2.x - center.x, 2f) + Mathf.Pow(p2.z - center.z, 2f) - r * r;
		float discriminant = b * b - 4 * a * c;
		float t1, t2;
		Vector3 shooterPosition = new Vector3(0f, 0f, 0f);
		if(discriminant >= 0){
			t1 = (-b + Mathf.Pow(discriminant, 0.5f)) / (2*a);
			t2 = (-b - Mathf.Pow(discriminant, 0.5f)) / (2*a);
			if(t1 >= 0 && t1 <= 1){
				shooterPosition.x = t1 * p1.x + (1 - t1) * p2.x;
				shooterPosition.z = t1 * p1.z + (1 - t1) * p2.z;
				shooterPosition.y = 11.25f;
			}
			else if(t2 >= 0 && t2 <= 1){
				shooterPosition.x = t2 * p1.x + (1 - t2) * p2.x;
				shooterPosition.z = t2 * p1.z + (1 - t2) * p2.z;
				shooterPosition.y = 11.25f;
			}
			else{
				shooterPosition.y = 0;
			}
		}
		else{
			shooterPosition.y = 0;
		}
		return shooterPosition;
	}
	
	Vector3 CircleCircleIntersetion(Vector3 p1, Vector3 p2, float r){
		float distance = Distance(p1, p2);
		Vector3 result = new Vector3(0f, 0f, 0f);
		if(distance < r * 2){
			result.y = 11.25f;
		}
		return result;
	}
	
	Vector3 ShiftDistance(Vector3 point, float slope, float dist){
		float temp = dist / Mathf.Pow(1f + slope * slope, 0.5f);
		return new Vector3(-temp + point.x, 11.25f, -temp * slope + point.z);
	}
	Vector3 ShiftDistance(Vector3 p1, Vector3 p2, float dist){
		Vector3 result = new Vector3(0f, 11.25f, 0f);
		float distance = Distance(p1, p2);
		result.x = p2.x + (p2.x - p1.x) / (distance * dist);
		result.z = p2.z + (p2.z - p1.z) / (distance * dist);
		return result;
	}
	
	bool TestRoute(Vector3 chess, Vector3 point, string name){
		float slope = Slope(chess, point);
		Vector3 testPoint1 = ShiftDistance(chess, -1f / slope, radius);
		Vector3 testPoint2 = ShiftDistance(point, -1f / slope, radius);
		Vector3 testPoint3 = ShiftDistance(chess, -1f / slope, -radius);
		Vector3 testPoint4 = ShiftDistance(point, -1f / slope, -radius);
		Debug.DrawLine(testPoint1, testPoint2, Color.red, 5f, false);
		Debug.DrawLine(testPoint3, testPoint4, Color.red, 5f, false);

		
		foreach(Transform child in m.redChess.transform){
			if(child.gameObject.name != name){
				Vector3 intersection1 = LineSegCircleIntersection(testPoint1, testPoint2, child.position, radius);
				Vector3 intersection2 = LineSegCircleIntersection(testPoint3, testPoint4, child.position, radius);
				Vector3 intersection3 = CircleCircleIntersetion(chess, child.position, radius);
				if(intersection1.y > 0 || intersection2.y > 0 || intersection3.y > 0){
					return false;
				}
			}
		}
		
		foreach(Transform child in m.blackChess.transform){
			if(child.gameObject.name != name){
				Vector3 intersection1 = LineSegCircleIntersection(testPoint1, testPoint2, child.position, radius);
				Vector3 intersection2 = LineSegCircleIntersection(testPoint3, testPoint4, child.position, radius);
				Vector3 intersection3 = CircleCircleIntersetion(chess, child.position, radius);
				if(intersection1.y > 0 || intersection2.y > 0 || intersection3.y > 0){
					return false;
				}
			}
		}
		return true;
	}
}