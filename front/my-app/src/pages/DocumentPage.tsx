import React, { useEffect, useState, useRef } from "react";
import * as signalR from '@microsoft/signalr';
import { Operation, OperationType } from '../utils/OperationService';
import { useParams } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { getDocumentById, DocumentContent } from '../services/documentService';
import diff_match_patch from 'diff-match-patch';

const dmp = new diff_match_patch();

const DocumentPage: React.FC = () => {
  const [connection, setConnection] = useState<signalR.HubConnection>();
  const [document, setDocument] = useState<DocumentContent>();
  const [documentContent, setDocumentContent] = useState("");

  const currentVersion = useRef(-1);
  const operationsList = useRef<Operation[]>([]);
  const lastSentOperationIndex = useRef(-1);
  const currentSentOperationIndex = useRef(-1);

  const { slug } = useParams();
  const { user } = useAuth();

  const maybeSendNextOperation = async (con: signalR.HubConnection|undefined) => {
    console.log("Trying to send next operation\n");

    if (!con)
    {
      console.log("Connection is not set");
      return;
    }

    if (currentSentOperationIndex.current != -1) 
    {
      console.log("Can not send operation because there is already a send operation");
      return;
    }

    if (!(lastSentOperationIndex.current + 1 < operationsList.current.length)) 
    {
      console.log("No operation to send");
      return;
    }

    currentSentOperationIndex.current = ++lastSentOperationIndex.current;
    const operation = operationsList.current[currentSentOperationIndex.current];
    await con.send("SendOperation", operation, currentVersion.current).then(() => console.log("Sent operation: ", operation));
  }

  const applyOperationToContent = (content: string, operation: Operation): string => {
    switch (operation.type) {
      case OperationType.Insert:
        return content.slice(0, operation.pos) + operation.text + content.slice(operation.pos);
      case OperationType.Delete:
        return content.slice(0, operation.pos) + content.slice(operation.pos + operation.text.length);
      default:
        console.error('Unknown operation type:', operation.type);
        return content;
    }
  };

  const handleReceivedAcknowledge = async (version: number, con: signalR.HubConnection) => {
    currentVersion.current = version;
    currentSentOperationIndex.current = -1;
    await maybeSendNextOperation(con);
  }

  const handleJoinedDocument = (operations: Operation[], version: number) => {
    currentVersion.current = version;
    if (operations.length == 0) {
      return;
    }

    for (const operation of operations) {
      setDocumentContent((oldContent) => applyOperationToContent(oldContent, operation));
    }
  }

  const handleReceivedOperation = (operation: Operation, version: number) => {
    const oldOp = operation;
    let i = lastSentOperationIndex.current;
    if (currentSentOperationIndex.current == -1)
    {
      i += 1;
    }

    const opList = operationsList.current;
    let transformedOp = operation;
    for (; i < opList.length; i++)
    {
      transformedOp = transformedOp.transform(opList[i], true);
      opList[i] = opList[i].transform(oldOp);
    }

    console.log("Transformed operation:", transformedOp);
    setDocumentContent((oldContent) => applyOperationToContent(oldContent, transformedOp));
    currentVersion.current = version;
  }

  useEffect(() => {
    if (!slug) return;

    const fetchDocument = async (id: number) => {
      const document = await getDocumentById(id);
      setDocumentContent(document.text);
      setDocument(document);
    };

    console.log("Fetching document\n");

    const id = parseInt(slug);
    if (isNaN(id)) return;
    fetchDocument(id);
  }, [])

  useEffect(() => {
    if (!document) {
      console.log("Waiting for document\n");
      return;
    }

    if (!user?.token) {
      console.log('Waiting for a user.token to create connection\n');
      return;
    }
    console.log(`Creating connection to hub with token ${user.token}\n`);

    // Don't like having this just as a mutable variable but we need a reference to the connection otherwise we would not be able to stop it
    let con: signalR.HubConnection;
    
    const createAndInitConnection = async () => {
        con = new signalR.HubConnectionBuilder()
        .withUrl('http://localhost:5019/hubs/documents',
          { accessTokenFactory: () => user.token }
        )
        .withAutomaticReconnect()
        .build();

      con.on("JoinedDocument", (operations: Operation[], version: number, documentID: number) => {
        console.log(`Joined document ${documentID}\n  Received version: ${version}\n  Received operations: ${operations}\n`);
        handleJoinedDocument(operations, version);
      })

      con.on("JoinDocumentError", (error: String, documentID: number) => {
        console.log(`Failed to join document with id: ${documentID}\n
                     With an error: ${error}`);
        // TODO: handle error
      })

      con.on("ReceivedAcknowledge", async (version: number) => {
        console.log(`Received acknowledge. New version: ${version}\n`);
        await handleReceivedAcknowledge(version, con);
      })

      con.on("ReceivedOperation", (operation: Operation, version: number) => {
        console.log("Received operation:", operation, "With version: ", version);
        handleReceivedOperation(operation, version);
      });

      await con.start();
      await con.invoke("JoinDocument", document.id, document.version);

      setConnection(con);
    }

    createAndInitConnection();
    return () => {
      console.log("Trying to stop connection\n");
      console.log("Connection:", con);
      if (con) {
        con.stop();
      }
    }

  }, [user, user?.token, document])


  if (!user?.token) return <div>Загрузка документа...</div>;
  if (!document) return <div>Загрузка документа...</div>;
  if (!connection) return <div>Загрузка документа...</div>;

  const detectOperations = (oldContent: string, newContent: string) => {
    const operations: Operation[] = [];

    const diffs = dmp.diff_main(oldContent, newContent);

    dmp.diff_cleanupSemantic(diffs);

    let currentPos = 0;

    diffs.forEach(([operation, text]) => {
      switch (operation) {
        case 0:
          currentPos += text.length;
          break;

        case 1:
          operations.push(Operation.createInsertOp(currentPos, text, user?.user.id || 0));
          currentPos += text.length;
          break;

        case -1:
          operations.push(Operation.createDeleteOp(currentPos, text, user?.user.id || 0));
          break;

        default:
          console.error('Unknown operation type');
      }
    });

    return operations;
  };

  const handleContentChange = async (e: React.ChangeEvent<HTMLTextAreaElement>) => {
    const newContent = e.target.value;
    setDocumentContent(newContent);

    const operations = detectOperations(documentContent, newContent);
    console.log("Generated  operations\n:", operations);
    const opList = operationsList.current;
    for (const operation of operations) 
    {
      opList.push(operation);
    }

    await maybeSendNextOperation(connection);
  };

  console.log("Render");

  return (
    <div className="flex-1 flex overflow-hidden min-h-[calc(100vh-120px)]">
      <div className="flex-1 bg-white p-6 shadow-md flex flex-col">
        <h1 className="text-2xl font-bold mb-4">Редактирование документа {document.documentName}</h1>
        <div className="border border-gray-300 p-4 flex-1 overflow-hidden">
          <textarea
            readOnly={false}
            className="w-full h-full border-0 focus:outline-none resize-none overflow:hidden"
            placeholder="Введите текст вашего документа..."
            value={documentContent}
            onChange={handleContentChange}
          />
        </div>
      </div>
    </div>
  );
}

export default DocumentPage;