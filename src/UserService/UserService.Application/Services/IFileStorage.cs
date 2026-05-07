namespace UserService.Application.Services;

public interface IFileStorage
{
    Task<Uri> SaveAsync(
        Stream content,
        string fileName,
        string path,
        string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Uri fileUri,
        CancellationToken cancellationToken = default);
}