namespace instaProj.Models
{
    public class Follow
    {
        public int Id { get; set; }
        public int User_Id_Followed { get; set; } //Seguidor
        public int User_Id_Following {  get; set; } //Seguindo
        public User? User_Followed { get; set; }
        public User? User_Following { get; set; }

        // select cont(*) from Follow where User_Id_Followed = 1 - User 1 tem x seguidores
    }
}
