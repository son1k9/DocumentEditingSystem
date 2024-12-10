import React from "react";
import { DocumentContent } from '../services/documentService';

interface DocumentContentComponentProps {
    document: DocumentContent;
    readOnly: boolean;
}

const DocumentContentComponent: React.FC<DocumentContentComponentProps>= ({document, readOnly}) => {
    return (<div className="flex-1 flex overflow-hidden min-h-[calc(100vh-120px)]">
        <div className="flex-1 bg-white p-6 shadow-md flex flex-col">
          <h1 className="text-2xl font-bold mb-4">Редактирование документа</h1>
          <div className="border border-gray-300 p-4 flex-1 overflow-auto">
            <textarea
              readOnly={readOnly}
              className="w-full h-full border-0 focus:outline-none resize-none"
              placeholder="Введите текст вашего документа..."
              value={document.text}
            />
          </div>
        </div>
        </div>
    );
}

export default DocumentContentComponent;
