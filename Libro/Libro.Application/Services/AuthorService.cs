using AutoMapper;
using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorService> _logger;

        public AuthorService(IAuthorRepository authorRepository, IMapper mapper, ILogger<AuthorService> logger)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // AuthorResponseDto without books
        public async Task<PaginatedResult<Author>> GetAllAuthorsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var paginatedResult = await _authorRepository.GetAllAuthorsAsync(pageNumber, pageSize);

                var authorDtos = _mapper.Map<IEnumerable<Author>>(paginatedResult.Items);

                return new PaginatedResult<Author>(authorDtos, paginatedResult.TotalCount, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in AuthorService while getting all authors.");
                throw;
            }
        }

        public async Task<Author> GetAuthorByIdAsync(int authorId)
        {
            try
            {
                var author = await _authorRepository.GetAuthorByIdAsync(authorId);
                return author;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in AuthorService while getting the author by ID.");
                throw;
            }
        }

        public async Task<Author> AddAuthorAsync(AuthorDto authorDto)
        {
            try
            {
                var author = _mapper.Map<Author>(authorDto);
                return await _authorRepository.AddAuthorAsync(author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in AuthorService while adding an author.");
                throw;
            }
        }

        public async Task UpdateAuthorAsync(int authorId, AuthorDto authorDto)
        {
            try
            {
                Author author = await _authorRepository.GetAuthorByIdAsync(authorId);

                if (author == null)
                    throw new ResourceNotFoundException("Author", "ID", authorId.ToString());

                author.Name = authorDto.Name;

                await _authorRepository.UpdateAuthorAsync(author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in AuthorService while updating an author.");
                throw;
            }
        }

        public async Task DeleteAuthorAsync(int authorId)
        {
            try
            {
                Author author = await _authorRepository.GetAuthorByIdAsync(authorId);
                if (author == null)
                {
                    throw new ResourceNotFoundException("Author", "ID", authorId.ToString());
                }
                await _authorRepository.DeleteAuthorAsync(author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in AuthorService while deleting an author.");
                throw;
            }
        }
    }
}
