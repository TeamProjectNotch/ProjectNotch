using UnityEngine;
public abstract class Peripheral : MonoBehaviour{

    public int id, version, manufacturer;
    //
    public DCPU_Controller dcpuController;

    public Peripheral(int id, int version, int manufacturer){
        this.id = id;
        this.version = version;
        this.manufacturer = manufacturer;
    }

    public abstract void sendInterrupt(DCPUState state);

    public abstract void updatePeripheral();

    public void setState(DCPU_Controller controller) { dcpuController = controller; }
}
