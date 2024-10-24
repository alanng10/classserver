namespace ClassServer.Console;

class Network : NetworkNetwork
{
    public override bool Init()
    {
        base.Init();
        this.InfraInfra = InfraInfra.This;
        this.TextInfra = TextInfra.This;
        this.ConsoleConsole = ConsoleConsole.This;

        this.StringComp = StringComp.This;

        this.Range = new Range();
        this.Range.Init();

        this.DataCount = -1;

        this.CountData = new Data();
        this.CountData.Count = sizeof(int);
        this.CountData.Init();

        this.CountRange = new Range();
        this.CountRange.Init();
        this.CountRange.Count = this.CountData.Count;
        return true;
    }

    public virtual Console Console { get; set; }
    protected virtual InfraInfra InfraInfra { get; set; }
    protected virtual TextInfra TextInfra { get; set; }
    protected virtual ConsoleConsole ConsoleConsole { get; set; }
    protected virtual StringComp StringComp { get; set; }
    protected virtual Range Range { get; set; }

    private int DataCount { get; set; }
    private Data CountData { get; set; }
    private Range CountRange { get; set; }
    
    public override bool StatusEvent()
    {
        NetworkStatusList statusList;
        statusList = this.NetworkStatusList;

        Console console;
        console = this.Console;

        NetworkStatus status;
        status = this.Status;

        if (!(status == statusList.NoError))
        {
            this.Close();
            console.Thread.Exit(100 + status.Index);
        }
        return true;
    }

    public override bool CaseEvent()
    {
        NetworkCaseList caseList;
        caseList = this.NetworkCaseList;

        NetworkCase cc;
        cc = this.Case;

        if (cc == caseList.Connected)
        {
            // this.Console.Log("ClassServer.Console:NetworkCaseState.Execute Connected");
        }

        if (cc == caseList.Unconnected)
        {
            // this.Console.Log("ClassServer.Console:NetworkCaseState.Execute Unconnected");
        }

        return true;
    }

    public override bool DataEvent()
    {
        // this.Console.Log("Network read Start");

        Console console;
        console = this.Console;

        Network network;
        network = console.Network;

        Stream stream;
        stream = network.Stream;

        long ka;
        ka = network.ReadyCount;

        int dataCount;
        dataCount = this.DataCount;

        bool b;
        b = (dataCount == -1);

        if (b)
        {
            int kk;
            kk = sizeof(int);

            if (ka < kk)
            {
                return true;
            }

            stream.Read(this.CountData, this.CountRange);

            uint u;
            u = this.InfraInfra.DataMidGet(this.CountData, 0);

            int ke;
            ke = (int)u;

            if (ke < 0)
            {
                this.Console.Log(this.TextInfra.S("Network received data count invalid"));
                return true;
            }

            // this.Console.Log("Network received data count: " + ke.ToString());

            this.DataCount = ke;

            dataCount = ke;

            ka = ka - kk;

            b = false;
        }

        if (!b)
        {
            if (ka < dataCount)
            {
                return true;
            }

            // this.Console.Log("Read Data Count: " + dataCount);

            Data data;
            data = new Data();
            data.Count = dataCount;
            data.Init();

            Range range;
            range = this.Range;

            range.Count = dataCount;

            // this.Console.Log("Read Before 1111");

            stream.Read(data, range);

            // this.Console.Log("Read After 1111");

            Data textData;
            textData = this.CreateTextData(data);

            long textCount;
            textCount = textData.Count / sizeof(uint);

            Text text;
            text = new Text();
            text.Init();
            text.Data = textData;
            text.Range = new Range();
            text.Range.Init();
            text.Range.Count = textCount;

            // this.Console.Log("Read Text: " + text);

            // this.Console.Log("ExecuteClass Before");

            data = this.Console.ExecuteClass(text);

            // this.Console.Log("ExecuteClass After");

            // this.Console.Log("Write Data Count: " + data.Count);

            range.Count = data.Count;

            // this.Console.Log("Write Before 1111");

            stream.Write(data, range);

            // this.Console.Log("Write After 1111");

            this.DataCount = -1;
        }

        // this.Console.Log("Network read End");
        return true;
    }

    protected virtual Data CreateTextData(Data data)
    {
        InfraInfra infraInfra;
        infraInfra = this.InfraInfra;

        long count;
        count = data.Count / sizeof(ushort);

        Data k;
        k = new Data();
        k.Count = count * sizeof(uint);
        k.Init();

        long i;
        i = 0;
        while (i < count)
        {
            long indexA;
            long indexB;
            indexA = i * sizeof(ushort);
            indexB = i * sizeof(uint);

            ushort kk;
            kk = infraInfra.DataShortGet(data, indexA);

            uint na;
            na = kk;

            infraInfra.DataCharSet(k, indexB, na);

            i = i + 1;
        }

        Data a;
        a = k;
        return a;
    }
}