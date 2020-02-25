using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticleType { Jump, WallJump }
public class SpriteSheetParticleController : MonoBehaviour {
	private static List<ParticleTypeController> controllers = new List<ParticleTypeController>();

	private static ParticleTypeController GetController(ParticleType _type) {
		for (int i = 0; i < controllers.Count; i++) {
			if (controllers[i].Type == _type) return controllers[i];
		}

		ParticleTypeController controller = new ParticleTypeController(_type);
		controllers.Add(controller);
		return controller;
	}

	public static void SpawnParticle(ParticleType _type, Vector3 pos, int direction=1) {
		SpriteSheetParticleController particle = GetController(_type).GetParticle();
		particle.Activate(pos);
		particle.transform.localScale = new Vector3(direction * Mathf.Abs(particle.transform.localScale.x), particle.transform.localScale.y, particle.transform.localScale.z);
	}

	private static void PutBack(SpriteSheetParticleController particle) {
		GetController(particle.type).PutBack(particle);
	}

	private ParticleType type;
	
	public void Set(ParticleType _type) { type = _type; }

	// Use this for initialization
	public void Activate(Vector3 pos) {
		transform.position = pos;
		gameObject.SetActive(true);
		GetComponent<Animator>().SetTrigger("Activate");
	}

	void ParticleEnd() {
		gameObject.SetActive(false);
		SpriteSheetParticleController.PutBack(this);
	}

	private class ParticleTypeController {
		private ParticleType type;
		public ParticleType Type { get { return type; } }
		private GameObject prefab;
		private List<SpriteSheetParticleController> instantiated;

		public ParticleTypeController(ParticleType _type) {
			type = _type;
			instantiated = new List<SpriteSheetParticleController>();
			prefab = Resources.Load<GameObject>("SpriteSheetParticle/" + type.ToString());
		}

		public SpriteSheetParticleController GetParticle() {
			if (instantiated.Count > 0) {
				SpriteSheetParticleController particle = instantiated[0];
				instantiated.RemoveAt(0);
				return particle;
			} else {
				SpriteSheetParticleController particle = Instantiate(prefab).GetComponent<SpriteSheetParticleController>();
				particle.Set(type);
				return particle;
			}
		}

		public void PutBack(SpriteSheetParticleController particle) {
			instantiated.Add(particle);
		}
	}
}
