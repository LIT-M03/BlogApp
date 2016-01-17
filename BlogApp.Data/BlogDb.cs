using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data
{
    public class BlogDb
    {
        private string _connectionString;
        public BlogDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Post> GetTop5Posts()
        {
            List<Post> posts = new List<Post>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP 5 * FROM Posts ORDER BY Date DESC";
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    posts.Add(GetPostFromReader(reader));
                }
            }

            return posts;
        }

        public Post GetPostById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Posts WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = cmd.ExecuteReader();
                reader.Read();
                return GetPostFromReader(reader);
            }
        }

        public void AddPost(Post post)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Posts (Content, Date, Title) VALUES (@content, @date, @title); SELECT @@Identity";
                cmd.Parameters.AddWithValue("@content", post.Content);
                cmd.Parameters.AddWithValue("@date", post.Date);
                cmd.Parameters.AddWithValue("@title", post.Title);
                connection.Open();
                post.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        public IEnumerable<Comment> GetCommentsForPost(int postId)
        {
            List<Comment> comments = new List<Comment>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Comments WHERE PostId = @postId";
                cmd.Parameters.AddWithValue("@postId", postId);
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Comment comment = new Comment
                    {
                        Content = (string)reader["Content"],
                        Date = (DateTime)reader["Date"],
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        PostId = (int)reader["PostId"]
                    };
                    comments.Add(comment);
                }
            }

            return comments;
        }

        public void AddComment(Comment comment)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Comments (Content, Date, Name, PostId) VALUES (@content, @date, @name, @postId); SELECT @@Identity";
                cmd.Parameters.AddWithValue("@content", comment.Content);
                cmd.Parameters.AddWithValue("@date", comment.Date);
                cmd.Parameters.AddWithValue("@name", comment.Name);
                cmd.Parameters.AddWithValue("@postId", comment.PostId);
                connection.Open();
                comment.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        public void AddTagsToPost(string tagsString, int postId)
        {
            string[] tags = tagsString.Split(',');
            
            IEnumerable<int> tagIds = tags.Select(AddTag);
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO PostsTags (PostId, TagId) VALUES (@postId, @tagId)";
                connection.Open();
                foreach (int tagId in tagIds)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@postId", postId);
                    cmd.Parameters.AddWithValue("@tagId", tagId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private int AddTag(string tag)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "AddTag";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", tag);
                connection.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        private Post GetPostFromReader(SqlDataReader reader)
        {
            return new Post
            {
                Content = (string)reader["Content"],
                Date = (DateTime)reader["Date"],
                Id = (int)reader["Id"],
                Title = (string)reader["Title"]
            };
        }
    }
}
