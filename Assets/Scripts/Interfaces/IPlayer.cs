using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Ensure you init SyncVars for use
interface IPlayer
{
    // We need to ensure all SyncVars are updated when players are initialized
    // Somehow the Syncvars refuse to sync on server and client so we ensure both works.
    void Init();
}

