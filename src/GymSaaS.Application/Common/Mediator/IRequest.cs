namespace GymSaaS.Application.Common.Mediator;

// Marker interface — just says "this is a message that expects a TResponse back."
public interface IRequest<out TResponse> { }