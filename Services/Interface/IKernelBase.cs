using Microsoft.SemanticKernel;

namespace sktodo.Services.Interface
{
    public interface IKernelBase{
        Kernel CreateKernel();
    }
}