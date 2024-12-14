using backend.DTO;

namespace backend.REPOSITORY
{
    public interface IArticleRepository
    {
        Task<IEnumerable<ArticleResponseDto>> GetArticlesAsync();
        Task<ArticleResponseDto?> GetArticleByIdAsync(int id);
        Task<ArticleResponseDto?> UpdateArticleAsync(int id, ArticleRequestDto articleRequestDto);
        Task<ArticleResponseDto> CreateArticleAsync(ArticleRequestDto articleRequestDto);

        Task<bool> DeleteArticleAsync(int id);

        Task<(IEnumerable<ArticleResponseDto> Articles, int TotalCount)> GetArticlesPaginateAsync(int page, int pageSize);

    }
}
