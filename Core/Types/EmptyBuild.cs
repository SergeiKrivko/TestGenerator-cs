using Shared;
using Shared.Utils;

namespace Core.Types;

public class EmptyBuild : BuildType
{
    public override string Command => "echo";
}