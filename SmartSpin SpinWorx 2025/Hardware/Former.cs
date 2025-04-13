using System.Text.Json.Nodes;

namespace SmartSpin.Hardware
{
    public class Former
    {
        public bool FormerAvail;

        public readonly ControllerAxis HAxis;

        public readonly ControllerAxis VAxis;

        public Former(JsonNode setupNode)
        {
            FormerAvail = (setupNode != null);
            if (FormerAvail)
            {
                if (setupNode["HAxis"] is JsonNode haxis)
                {
                    HAxis = Machine.Axes[(int)haxis];
                }
                if (setupNode["VAxis"] is JsonNode vaxis)
                {
                    HAxis = Machine.Axes[(int)vaxis];
                }
            }
        }

    }
}
