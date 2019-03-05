using System;
using UnityEngine;

public class Coordinates {

	public static Coordinates zero 	= new Coordinates(0,0);
	public static Coordinates one 	= new Coordinates(1,1);
	public static Coordinates up 	= new Coordinates(0,1);
	public static Coordinates down 	= new Coordinates(0,-1);
	public static Coordinates left 	= new Coordinates(-1,0);
	public static Coordinates right = new Coordinates(1,0);

	int x;
	public int X { 
		get { 
			return x; 
		} 
		set {
			this.x = value;
		}
	}

	int y;
	public int Y { 
		get { 
			return y; 
		} 
		set {
			this.y = value;
		}
	}

	public int[] coordinates;

	public Coordinates(int _x, int _y){
		coordinates = new int[2];
		x = coordinates[0] = _x;
		y = coordinates[1] = _y;
	}

	public Coordinates(float _x, float _y){
		coordinates = new int[2];
		x = coordinates[0] = Mathf.RoundToInt(_x);
		y = coordinates[1] = Mathf.RoundToInt(_y);
	}

	public int this[int i] {
		get {
			return coordinates[i];
		}
	}

	public override bool Equals(System.Object obj) {
		if (obj == null || GetType() != obj.GetType()) {
			return false;
		}

		Coordinates c = (Coordinates)obj;
		return (x == c.x) && (y == c.y);
	}

	public override int GetHashCode() {
		return x + y * 255;
	}

	public static Coordinates operator +(Coordinates c1, Coordinates c2) {
		return new Coordinates(c1.x + c2.x, c1.y + c2.y);
	}

	public static Coordinates operator -(Coordinates c1, Coordinates c2) {
		return new Coordinates(c1.x - c2.x, c1.y - c2.y);
	}

	public static Coordinates operator *(Coordinates c1, int s) {
		return new Coordinates(c1.x * s, c1.y * s);
	}

	public static bool operator ==(Coordinates c1, Coordinates c2) {
		return c1.x == c2.x && c1.y == c2.y;
	}

	public static bool operator !=(Coordinates c1, Coordinates c2) {
		return c1.x != c2.x || c1.y != c2.y;
	}

	public override string ToString() {
		return x + " : " + y; 
	}

	public static implicit operator Vector3(Coordinates c1) {
		return new Vector3(c1.x,c1.y,0);
	}

	public static implicit operator Vector2(Coordinates c1) {
		return new Vector3(c1.x,c1.y);
	}

	public static implicit operator Coordinates(Vector3 v1) {
		return new Coordinates(v1.x,v1.y);
	}

	public static implicit operator Coordinates(Vector2 v1) {
		return new Coordinates(v1.x,v1.y);
	}
}
