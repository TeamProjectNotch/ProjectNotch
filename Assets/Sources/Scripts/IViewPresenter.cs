using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public interface IViewPresenter {

	Vector3 position {get; set;}
	Quaternion rotation {get; set;}

	void Link(IEntity entity, IContext context);
	void Unlink();

	void Remove();
}
