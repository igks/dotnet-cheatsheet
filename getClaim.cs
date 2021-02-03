private int getUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type.Equals("Id", StringComparison.InvariantCultureIgnoreCase));
            if (idClaim != null)
            {
                var id = int.Parse(idClaim.Value);
                return id;
            }
            return -1;
        }
