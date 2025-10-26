using MediatR;
using SharedKernel;

namespace Application.Abstractions.Messaging;

// Application-level marker indicating that the command should run in a transaction.
public interface ITransactionalCommand : ICommand;

public interface ITransactionalCommand<TResponse> : IRequest<Result<TResponse>>, ITransactionalCommand;


//  Transaction Pipeline behavior example:
// public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//     where TRequest : IRequest<TResponse>
// {
//     private readonly IUnitOfWork _unitOfWork;

//     public TransactionBehavior(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

//     public async Task<TResponse> Handle(
//         TRequest request,
//         CancellationToken cancellationToken,
//         RequestHandlerDelegate<TResponse> next)
//     {
//         if (request is ITransactionalCommand)
//         {
//             await using var transaction = await _unitOfWork.BeginTransactionAsync();
//             var response = await next();
//             await _unitOfWork.SaveChangesAsync(cancellationToken);
//             transaction.Commit();
//             return response;
//         }

//         return await next();
//     }
// }