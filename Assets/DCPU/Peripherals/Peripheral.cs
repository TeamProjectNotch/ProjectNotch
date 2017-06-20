public abstract class Peripheral{

    public int id, version, manufacturer;

    public Peripheral(int id, int version, int manufacturer){
        this.id = id;
        this.version = version;
        this.manufacturer = manufacturer;
    }

    public abstract void sendInterrupt(dcpuState state);

    public abstract void step();
}
