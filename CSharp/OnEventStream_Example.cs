using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;


    public class WorkflowAttachment
    {
        private static readonly global::Common.Logging.ILog Log = global::Common.Logging.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async System.Threading.Tasks.Task<System.IO.Stream> GetWorkflowAttachmentData(int attachmentid)
        {


                //... Setup SqlCommand 1st return Blob or SQLFileStream

                // The reader needs to be executed with the SequentialAccess behavior to enable network streaming
                // Otherwise ReadAsync will buffer the entire BLOB into memory which can cause scalability issues or even OutOfMemoryExceptions
                SqlDataReader reader = await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.SequentialAccess);

                if (await reader.ReadAsync())
                {
                    if (!(await reader.IsDBNullAsync(0)))
                    {
                        return new Lib.OnEventStream(reader.GetStream(0),
                            OnClosed: (Lib.OnEventStream stream) =>
                            {
                                Log.Debug("Stream Close; Disposing DB items.");
                                reader.Dispose();
                                cmd.Dispose();
                                db.Dispose();
                            }
                            );
                    }
                }
            }
            throw new ApplicationException("GetWorkflowAttachmentData not there.");
        }


    }
