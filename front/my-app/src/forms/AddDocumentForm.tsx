import React, { useState }from "react";
import { useForm } from "react-hook-form";
import { addDocument } from "../services/documentService";

interface AddDocumentProps{
    closeModal: () => void;
    refreshDocuments: () => void;
}

const AddDocumentForm: React.FC<AddDocumentProps> = ({closeModal, refreshDocuments}) => {
    const { register, handleSubmit } = useForm();
    const [file, setFile] = useState<File | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const onSubmit = async (data : any) => {
        setIsSubmitting(true);
        const formData = new FormData();

        if (file){
            formData.append('file', file);
        }else{
            const emptyFile = new Blob([''], { type: 'text/plain' });
            formData.append('file', emptyFile, 'empty.txt');
        }

        try
        {
            await addDocument(formData);
            refreshDocuments();
            closeModal();
        }catch(error){
            console.error('Error adding document', error);
        }finally{
            setIsSubmitting(false);
        }
    };

    return(
        <div className="modal-overlay">
        <div className="modal-content">
            <button
            className="modal-close-button absolute top-0 right-2 text-lg text-gray-600"
            onClick={closeModal}
            >
            &times; 
            </button>
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div className="m-5">
                <label>Загрузите txt файл, для редактирования:</label>
                <input
                type="file"
                accept=".txt"
                onChange={(e) => setFile(e.target.files?.[0] || null)}
                className="input mt-3"
                />
            </div>
            <div className="flex justify-end">
                <button type="submit" className="btn bg-blue-500 p-2 rounded-full" disabled={isSubmitting}>
                {isSubmitting ? 'Submitting...' : 'Add Document'}
                </button>
            </div>
            </form>
        </div>
        </div>
    )
}

export default AddDocumentForm;